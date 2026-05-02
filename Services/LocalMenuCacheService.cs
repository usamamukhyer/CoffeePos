using System.Text.Json;
using DBCafeteria.Models;
using Microsoft.Data.Sqlite;

namespace DBCafeteria.Services;

public sealed class LocalMenuCacheService
{
    private readonly LocalCacheDatabase _database = LocalCacheDatabase.Instance;

    public async Task<IReadOnlyList<CategoryModel>> GetCategoriesAsync(CancellationToken cancellationToken = default) =>
        await _database.QueryAsync<CategoryModel>(async (connection, token) =>
        {
            var results = new List<CategoryModel>();
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Key FROM Categories ORDER BY Name;";
            await using var reader = await command.ExecuteReaderAsync(token);
            while (await reader.ReadAsync(token))
            {
                results.Add(new CategoryModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Key = reader.GetString(2),
                    Image = GetCategoryImage(reader.GetString(1))
                });
            }

            return results;
        }, cancellationToken);

    public async Task<IReadOnlyList<ProductModel>> GetProductsAsync(int categoryId, CancellationToken cancellationToken = default) =>
        await _database.QueryAsync<ProductModel>(async (connection, token) =>
        {
            var results = new List<ProductModel>();
            await using var command = connection.CreateCommand();
            command.CommandText = """
                SELECT Id, CategoryId, Name, HotAvailable, ColdAvailable, Price, PriceWithTax
                FROM Products
                WHERE CategoryId = $categoryId
                ORDER BY Name;
                """;
            command.Parameters.Add(LocalCacheDatabase.Parameter("$categoryId", categoryId));
            await using var reader = await command.ExecuteReaderAsync(token);
            while (await reader.ReadAsync(token))
            {
                var name = reader.GetString(2);
                results.Add(new ProductModel
                {
                    Id = reader.GetInt32(0),
                    CategoryId = reader.GetInt32(1),
                    Name = name,
                    HotAvailable = reader.GetInt32(3) == 1,
                    ColdAvailable = reader.GetInt32(4) == 1,
                    BasePrice = (decimal)(ReadNullableDouble(reader, 6) ?? ReadNullableDouble(reader, 5) ?? 0),
                    Image = GetProductImage(name)
                });
            }

            return results;
        }, cancellationToken);

    public async Task<ApiCustomization?> GetCustomizationAsync(int productId, CancellationToken cancellationToken = default) =>
        await _database.ExecuteAsync(async (connection, token) =>
        {
            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT PayloadJson FROM Customizations WHERE ProductId = $productId;";
            command.Parameters.Add(LocalCacheDatabase.Parameter("$productId", productId));
            var payload = await command.ExecuteScalarAsync(token) as string;
            return string.IsNullOrWhiteSpace(payload) ? null : JsonSerializer.Deserialize<ApiCustomization>(payload);
        }, cancellationToken);

    public async Task UpsertCategoriesAsync(IEnumerable<ApiCategory> categories, CancellationToken cancellationToken = default) =>
        await _database.ExecuteAsync(async (connection, token) =>
        {
            foreach (var category in categories)
            {
                var version = VersionOf(category.Id, category.Name, category.Key, category.Price, category.PriceWithTax);
                await UpsertIfChangedAsync(connection, "Categories", category.Id, version, """
                    INSERT INTO Categories (Id, Name, Key, Price, PriceWithTax, Version, UpdatedAt)
                    VALUES ($id, $name, $key, $price, $priceWithTax, $version, $updatedAt)
                    ON CONFLICT(Id) DO UPDATE SET
                        Name = excluded.Name,
                        Key = excluded.Key,
                        Price = excluded.Price,
                        PriceWithTax = excluded.PriceWithTax,
                        Version = excluded.Version,
                        UpdatedAt = excluded.UpdatedAt;
                    """, token,
                    LocalCacheDatabase.Parameter("$id", category.Id),
                    LocalCacheDatabase.Parameter("$name", category.Name),
                    LocalCacheDatabase.Parameter("$key", category.Key),
                    LocalCacheDatabase.Parameter("$price", category.Price),
                    LocalCacheDatabase.Parameter("$priceWithTax", category.PriceWithTax),
                    LocalCacheDatabase.Parameter("$version", version),
                    LocalCacheDatabase.Parameter("$updatedAt", DateTime.UtcNow.ToString("O")));
            }
        }, cancellationToken);

    public async Task UpsertProductsAsync(IEnumerable<ApiProduct> products, CancellationToken cancellationToken = default) =>
        await _database.ExecuteAsync(async (connection, token) =>
        {
            foreach (var product in products)
            {
                var version = VersionOf(product.Id, product.CategoryId, product.Name, product.HotAvailable, product.ColdAvailable, product.Price, product.PriceWithTax);
                await UpsertIfChangedAsync(connection, "Products", product.Id, version, """
                    INSERT INTO Products (Id, CategoryId, Name, HotAvailable, ColdAvailable, Price, PriceWithTax, Version, UpdatedAt)
                    VALUES ($id, $categoryId, $name, $hotAvailable, $coldAvailable, $price, $priceWithTax, $version, $updatedAt)
                    ON CONFLICT(Id) DO UPDATE SET
                        CategoryId = excluded.CategoryId,
                        Name = excluded.Name,
                        HotAvailable = excluded.HotAvailable,
                        ColdAvailable = excluded.ColdAvailable,
                        Price = excluded.Price,
                        PriceWithTax = excluded.PriceWithTax,
                        Version = excluded.Version,
                        UpdatedAt = excluded.UpdatedAt;
                    """, token,
                    LocalCacheDatabase.Parameter("$id", product.Id),
                    LocalCacheDatabase.Parameter("$categoryId", product.CategoryId),
                    LocalCacheDatabase.Parameter("$name", product.Name),
                    LocalCacheDatabase.Parameter("$hotAvailable", product.HotAvailable ? 1 : 0),
                    LocalCacheDatabase.Parameter("$coldAvailable", product.ColdAvailable ? 1 : 0),
                    LocalCacheDatabase.Parameter("$price", product.Price),
                    LocalCacheDatabase.Parameter("$priceWithTax", product.PriceWithTax),
                    LocalCacheDatabase.Parameter("$version", version),
                    LocalCacheDatabase.Parameter("$updatedAt", DateTime.UtcNow.ToString("O")));
            }
        }, cancellationToken);

    public async Task UpsertCustomizationAsync(int productId, ApiCustomization customization, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(customization);
        var version = VersionOf(productId, payload);
        await _database.ExecuteAsync(async (connection, token) =>
        {
            await UpsertIfChangedAsync(connection, "Customizations", productId, version, """
                INSERT INTO Customizations (ProductId, PayloadJson, Version, UpdatedAt)
                VALUES ($productId, $payloadJson, $version, $updatedAt)
                ON CONFLICT(ProductId) DO UPDATE SET
                    PayloadJson = excluded.PayloadJson,
                    Version = excluded.Version,
                    UpdatedAt = excluded.UpdatedAt;
                """, token,
                LocalCacheDatabase.Parameter("$productId", productId),
                LocalCacheDatabase.Parameter("$payloadJson", payload),
                LocalCacheDatabase.Parameter("$version", version),
                LocalCacheDatabase.Parameter("$updatedAt", DateTime.UtcNow.ToString("O")));
        }, cancellationToken);
    }

    public async Task UpsertOptionsAsync(string optionType, IEnumerable<ApiOption> options, CancellationToken cancellationToken = default) =>
        await _database.ExecuteAsync(async (connection, token) =>
        {
            foreach (var option in options)
            {
                var version = VersionOf(optionType, option.Id, option.Name, option.Price, option.PriceWithTax);
                await using var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO Options (OptionType, Id, Name, Price, PriceWithTax, Version, UpdatedAt)
                    VALUES ($optionType, $id, $name, $price, $priceWithTax, $version, $updatedAt)
                    ON CONFLICT(OptionType, Id) DO UPDATE SET
                        Name = CASE WHEN Options.Version <> excluded.Version THEN excluded.Name ELSE Options.Name END,
                        Price = CASE WHEN Options.Version <> excluded.Version THEN excluded.Price ELSE Options.Price END,
                        PriceWithTax = CASE WHEN Options.Version <> excluded.Version THEN excluded.PriceWithTax ELSE Options.PriceWithTax END,
                        Version = excluded.Version,
                        UpdatedAt = CASE WHEN Options.Version <> excluded.Version THEN excluded.UpdatedAt ELSE Options.UpdatedAt END;
                    """;
                command.Parameters.AddRange([
                    LocalCacheDatabase.Parameter("$optionType", optionType),
                    LocalCacheDatabase.Parameter("$id", option.Id),
                    LocalCacheDatabase.Parameter("$name", option.Name),
                    LocalCacheDatabase.Parameter("$price", option.Price),
                    LocalCacheDatabase.Parameter("$priceWithTax", option.PriceWithTax),
                    LocalCacheDatabase.Parameter("$version", version),
                    LocalCacheDatabase.Parameter("$updatedAt", DateTime.UtcNow.ToString("O"))
                ]);
                await command.ExecuteNonQueryAsync(token);
            }
        }, cancellationToken);

    private static async Task UpsertIfChangedAsync(SqliteConnection connection, string tableName, int id, string version, string sql, CancellationToken cancellationToken, params SqliteParameter[] parameters)
    {
        await using var versionCommand = connection.CreateCommand();
        var keyColumn = tableName == "Customizations" ? "ProductId" : "Id";
        versionCommand.CommandText = $"SELECT Version FROM {tableName} WHERE {keyColumn} = $id;";
        versionCommand.Parameters.Add(LocalCacheDatabase.Parameter("$id", id));
        var currentVersion = await versionCommand.ExecuteScalarAsync(cancellationToken) as string;
        if (currentVersion == version)
            return;

        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static double? ReadNullableDouble(SqliteDataReader reader, int ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetDouble(ordinal);

    private static string VersionOf(params object?[] values) => string.Join("|", values.Select(value => value?.ToString() ?? string.Empty));

    private static string GetCategoryImage(string name)
    {
        if (name.Contains("tea", StringComparison.OrdinalIgnoreCase))
            return "coffee_tea.png";
        if (name.Contains("frappe", StringComparison.OrdinalIgnoreCase))
            return "coffee_frappe.png";
        if (name.Contains("chocolate", StringComparison.OrdinalIgnoreCase))
            return "coffee_chocolate.png";

        return "coffee_cup.png";
    }

    private static string GetProductImage(string name)
    {
        if (name.Contains("latte", StringComparison.OrdinalIgnoreCase))
            return "coffee_latte.png";
        if (name.Contains("espresso", StringComparison.OrdinalIgnoreCase))
            return "coffee_espresso.png";
        if (name.Contains("cappuccino", StringComparison.OrdinalIgnoreCase))
            return "coffee_cappuccino.png";
        if (name.Contains("americano", StringComparison.OrdinalIgnoreCase))
            return "coffee_americano.png";

        return "coffee_cup.png";
    }
}
