using System.Text.Json;
using Microsoft.Maui.Networking;

namespace DBCafeteria.Services;

public sealed class SyncQueueService
{
    private readonly LocalCacheDatabase _database = LocalCacheDatabase.Instance;
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();

    public async Task EnqueueOrderAsync(string clientOrderId, ApiOrderRequest request, string status, string? error = null, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(request);
        var now = DateTime.UtcNow.ToString("O");
        await _database.ExecuteAsync(async (connection, token) =>
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO SyncQueue (ClientOrderId, PayloadJson, Status, AttemptCount, CreatedAt, UpdatedAt, LastError)
                VALUES ($clientOrderId, $payloadJson, $status, 0, $createdAt, $updatedAt, $lastError)
                ON CONFLICT(ClientOrderId) DO UPDATE SET
                    PayloadJson = excluded.PayloadJson,
                    Status = excluded.Status,
                    UpdatedAt = excluded.UpdatedAt,
                    LastError = excluded.LastError;
                """;
            command.Parameters.AddRange([
                LocalCacheDatabase.Parameter("$clientOrderId", clientOrderId),
                LocalCacheDatabase.Parameter("$payloadJson", payload),
                LocalCacheDatabase.Parameter("$status", status),
                LocalCacheDatabase.Parameter("$createdAt", now),
                LocalCacheDatabase.Parameter("$updatedAt", now),
                LocalCacheDatabase.Parameter("$lastError", error)
            ]);
            await command.ExecuteNonQueryAsync(token);
        }, cancellationToken);
    }

    public async Task MarkSyncedAsync(string clientOrderId, CancellationToken cancellationToken = default)
    {
        await _database.ExecuteAsync(async (connection, token) =>
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE SyncQueue
                SET Status = 'Synced', UpdatedAt = $updatedAt, LastError = NULL
                WHERE ClientOrderId = $clientOrderId;
                """;
            command.Parameters.AddRange([
                LocalCacheDatabase.Parameter("$clientOrderId", clientOrderId),
                LocalCacheDatabase.Parameter("$updatedAt", DateTime.UtcNow.ToString("O"))
            ]);
            await command.ExecuteNonQueryAsync(token);
        }, cancellationToken);
    }

    public async Task FlushPendingOrdersAsync(CancellationToken cancellationToken = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return;

        var pending = await GetPendingOrdersAsync(cancellationToken);
        foreach (var item in pending)
        {
            try
            {
                var request = JsonSerializer.Deserialize<ApiOrderRequest>(item.PayloadJson);
                if (request is null)
                    continue;

                await _apiClient.PostAsync<ApiOrderRequest, ApiOrderResponse>("orders/place-order", request, cancellationToken);
                await MarkSyncedAsync(item.ClientOrderId, cancellationToken);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or ApiException)
            {
                await MarkRetryAsync(item.ClientOrderId, ex.Message, cancellationToken);
            }
        }
    }

    private async Task<IReadOnlyList<LocalSyncQueueRow>> GetPendingOrdersAsync(CancellationToken cancellationToken) =>
        await _database.QueryAsync<LocalSyncQueueRow>(async (connection, token) =>
        {
            var results = new List<LocalSyncQueueRow>();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT ClientOrderId, PayloadJson, Status, AttemptCount, CreatedAt, UpdatedAt, LastError
                FROM SyncQueue
                WHERE Status IN ('PendingSync', 'PendingServer')
                ORDER BY CreatedAt;
                """;
            await using var reader = await command.ExecuteReaderAsync(token);
            while (await reader.ReadAsync(token))
            {
                results.Add(new LocalSyncQueueRow(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetInt32(3),
                    DateTime.Parse(reader.GetString(4)),
                    DateTime.Parse(reader.GetString(5)),
                    reader.IsDBNull(6) ? null : reader.GetString(6)));
            }

            return results;
        }, cancellationToken);

    private async Task MarkRetryAsync(string clientOrderId, string error, CancellationToken cancellationToken) =>
        await _database.ExecuteAsync(async (connection, token) =>
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE SyncQueue
                SET AttemptCount = AttemptCount + 1,
                    UpdatedAt = $updatedAt,
                    LastError = $lastError
                WHERE ClientOrderId = $clientOrderId;
                """;
            command.Parameters.AddRange([
                LocalCacheDatabase.Parameter("$clientOrderId", clientOrderId),
                LocalCacheDatabase.Parameter("$updatedAt", DateTime.UtcNow.ToString("O")),
                LocalCacheDatabase.Parameter("$lastError", error)
            ]);
            await command.ExecuteNonQueryAsync(token);
        }, cancellationToken);
}
