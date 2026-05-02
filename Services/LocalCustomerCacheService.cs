using DBCafeteria.Models;

namespace DBCafeteria.Services;

public sealed class LocalCustomerCacheService
{
    private readonly LocalCacheDatabase _database = LocalCacheDatabase.Instance;

    public async Task<IReadOnlyList<CustomerSearchResultModel>> SearchCustomersAsync(string query, int? excludedCustomerId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return [];

        var term = $"%{query.Trim()}%";
        return await _database.QueryAsync<CustomerSearchResultModel>(async (connection, token) =>
        {
            var results = new List<CustomerSearchResultModel>();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT Id, Name, Phone, Email
                FROM Customers
                WHERE Active = 1
                  AND Name LIKE $term
                  AND ($excludedCustomerId IS NULL OR Id <> $excludedCustomerId)
                ORDER BY Name
                LIMIT 10;
                """;
            command.Parameters.Add(LocalCacheDatabase.Parameter("$term", term));
            command.Parameters.Add(LocalCacheDatabase.Parameter("$excludedCustomerId", excludedCustomerId));
            await using var reader = await command.ExecuteReaderAsync(token);
            while (await reader.ReadAsync(token))
            {
                results.Add(new CustomerSearchResultModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Phone = reader.GetString(2),
                    Email = reader.IsDBNull(3) ? null : reader.GetString(3)
                });
            }

            return results;
        }, cancellationToken);
    }

    public async Task UpsertCustomersAsync(IEnumerable<ApiCustomerSyncItem> customers, CancellationToken cancellationToken = default) =>
        await _database.ExecuteAsync(async (connection, token) =>
        {
            foreach (var customer in customers)
            {
                await using var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO Customers (Id, Name, Phone, Email, IsGuest, Active, Version, UpdatedAt)
                    VALUES ($id, $name, $phone, $email, $isGuest, $active, $version, $updatedAt)
                    ON CONFLICT(Id) DO UPDATE SET
                        Name = CASE WHEN Customers.Version <> excluded.Version THEN excluded.Name ELSE Customers.Name END,
                        Phone = CASE WHEN Customers.Version <> excluded.Version THEN excluded.Phone ELSE Customers.Phone END,
                        Email = CASE WHEN Customers.Version <> excluded.Version THEN excluded.Email ELSE Customers.Email END,
                        IsGuest = CASE WHEN Customers.Version <> excluded.Version THEN excluded.IsGuest ELSE Customers.IsGuest END,
                        Active = CASE WHEN Customers.Version <> excluded.Version THEN excluded.Active ELSE Customers.Active END,
                        Version = excluded.Version,
                        UpdatedAt = CASE WHEN Customers.Version <> excluded.Version THEN excluded.UpdatedAt ELSE Customers.UpdatedAt END;
                    """;
                command.Parameters.AddRange([
                    LocalCacheDatabase.Parameter("$id", customer.Id),
                    LocalCacheDatabase.Parameter("$name", customer.Name),
                    LocalCacheDatabase.Parameter("$phone", customer.Phone),
                    LocalCacheDatabase.Parameter("$email", customer.Email),
                    LocalCacheDatabase.Parameter("$isGuest", customer.IsGuest ? 1 : 0),
                    LocalCacheDatabase.Parameter("$active", customer.Active ? 1 : 0),
                    LocalCacheDatabase.Parameter("$version", customer.Version),
                    LocalCacheDatabase.Parameter("$updatedAt", DateTime.UtcNow.ToString("O"))
                ]);
                await command.ExecuteNonQueryAsync(token);
            }
        }, cancellationToken);
}
