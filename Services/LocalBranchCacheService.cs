using DBCafeteria.Models;

namespace DBCafeteria.Services;

public sealed class LocalBranchCacheService
{
    private readonly LocalCacheDatabase _database = LocalCacheDatabase.Instance;

    public async Task<IReadOnlyList<BranchModel>> GetBranchesAsync(CancellationToken cancellationToken = default) =>
        await _database.QueryAsync<BranchModel>(async (connection, token) =>
        {
            var results = new List<BranchModel>();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Address FROM Branches ORDER BY Name;";
            await using var reader = await command.ExecuteReaderAsync(token);
            while (await reader.ReadAsync(token))
            {
                results.Add(new BranchModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Address = reader.IsDBNull(2) ? null : reader.GetString(2)
                });
            }

            return results;
        }, cancellationToken);

    public async Task UpsertBranchesAsync(IEnumerable<ApiBranch> branches, CancellationToken cancellationToken = default) =>
        await _database.ExecuteAsync(async (connection, token) =>
        {
            foreach (var branch in branches)
            {
                var version = $"{branch.Id}|{branch.Name}|{branch.Address}";
                await using var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO Branches (Id, Name, Address, Version, UpdatedAt)
                    VALUES ($id, $name, $address, $version, $updatedAt)
                    ON CONFLICT(Id) DO UPDATE SET
                        Name = CASE WHEN Branches.Version <> excluded.Version THEN excluded.Name ELSE Branches.Name END,
                        Address = CASE WHEN Branches.Version <> excluded.Version THEN excluded.Address ELSE Branches.Address END,
                        Version = excluded.Version,
                        UpdatedAt = CASE WHEN Branches.Version <> excluded.Version THEN excluded.UpdatedAt ELSE Branches.UpdatedAt END;
                    """;
                command.Parameters.AddRange([
                    LocalCacheDatabase.Parameter("$id", branch.Id),
                    LocalCacheDatabase.Parameter("$name", branch.Name),
                    LocalCacheDatabase.Parameter("$address", branch.Address),
                    LocalCacheDatabase.Parameter("$version", version),
                    LocalCacheDatabase.Parameter("$updatedAt", DateTime.UtcNow.ToString("O"))
                ]);
                await command.ExecuteNonQueryAsync(token);
            }
        }, cancellationToken);
}
