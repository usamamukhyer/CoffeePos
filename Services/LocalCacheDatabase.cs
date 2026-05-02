using Microsoft.Data.Sqlite;

namespace DBCafeteria.Services;

public sealed class LocalCacheDatabase
{
    public static LocalCacheDatabase Instance { get; } = new();

    private readonly SemaphoreSlim _gate = new(1, 1);
    private bool _initialized;

    private LocalCacheDatabase()
    {
    }

    private static string DatabasePath => Path.Combine(FileSystem.AppDataDirectory, "dbcafeteria-cache.db3");
    private static string ConnectionString => new SqliteConnectionStringBuilder { DataSource = DatabasePath }.ToString();

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (_initialized)
            return;

        await _gate.WaitAsync(cancellationToken);
        try
        {
            if (_initialized)
                return;

            Directory.CreateDirectory(FileSystem.AppDataDirectory);
            await using var connection = OpenConnection();
            await ExecuteNonQueryAsync(connection, """
                CREATE TABLE IF NOT EXISTS Categories (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Key TEXT NOT NULL,
                    Price REAL NULL,
                    PriceWithTax REAL NULL,
                    Version TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                );
                """, cancellationToken);
            await ExecuteNonQueryAsync(connection, """
                CREATE TABLE IF NOT EXISTS Products (
                    Id INTEGER PRIMARY KEY,
                    CategoryId INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    HotAvailable INTEGER NOT NULL,
                    ColdAvailable INTEGER NOT NULL,
                    Price REAL NULL,
                    PriceWithTax REAL NULL,
                    Version TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                );
                """, cancellationToken);
            await ExecuteNonQueryAsync(connection, "CREATE INDEX IF NOT EXISTS IX_Products_CategoryId ON Products(CategoryId);", cancellationToken);
            await ExecuteNonQueryAsync(connection, """
                CREATE TABLE IF NOT EXISTS Options (
                    OptionType TEXT NOT NULL,
                    Id INTEGER NOT NULL,
                    Name TEXT NOT NULL,
                    Price REAL NOT NULL,
                    PriceWithTax REAL NOT NULL,
                    Version TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL,
                    PRIMARY KEY (OptionType, Id)
                );
                """, cancellationToken);
            await ExecuteNonQueryAsync(connection, """
                CREATE TABLE IF NOT EXISTS Customizations (
                    ProductId INTEGER PRIMARY KEY,
                    PayloadJson TEXT NOT NULL,
                    Version TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                );
                """, cancellationToken);
            await ExecuteNonQueryAsync(connection, """
                CREATE TABLE IF NOT EXISTS Branches (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Address TEXT NULL,
                    Version TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                );
                """, cancellationToken);
            await ExecuteNonQueryAsync(connection, """
                CREATE TABLE IF NOT EXISTS Customers (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Phone TEXT NOT NULL,
                    Email TEXT NULL,
                    IsGuest INTEGER NOT NULL,
                    Active INTEGER NOT NULL,
                    Version TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL
                );
                """, cancellationToken);
            await ExecuteNonQueryAsync(connection, "CREATE INDEX IF NOT EXISTS IX_Customers_Name ON Customers(Name);", cancellationToken);
            await ExecuteNonQueryAsync(connection, """
                CREATE TABLE IF NOT EXISTS SyncQueue (
                    ClientOrderId TEXT PRIMARY KEY,
                    PayloadJson TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    AttemptCount INTEGER NOT NULL,
                    CreatedAt TEXT NOT NULL,
                    UpdatedAt TEXT NOT NULL,
                    LastError TEXT NULL
                );
                """, cancellationToken);
            _initialized = true;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<T>> QueryAsync<T>(Func<SqliteConnection, CancellationToken, Task<IReadOnlyList<T>>> query, CancellationToken cancellationToken = default)
    {
        await InitializeAsync(cancellationToken);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using var connection = OpenConnection();
            return await query(connection, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<T> ExecuteAsync<T>(Func<SqliteConnection, CancellationToken, Task<T>> action, CancellationToken cancellationToken = default)
    {
        await InitializeAsync(cancellationToken);
        await _gate.WaitAsync(cancellationToken);
        try
        {
            await using var connection = OpenConnection();
            return await action(connection, cancellationToken);
        }
        finally
        {
            _gate.Release();
        }
    }

    public Task ExecuteAsync(Func<SqliteConnection, CancellationToken, Task> action, CancellationToken cancellationToken = default) =>
        ExecuteAsync(async (connection, token) =>
        {
            await action(connection, token);
            return true;
        }, cancellationToken);

    public static async Task ExecuteNonQueryAsync(SqliteConnection connection, string sql, CancellationToken cancellationToken = default, params SqliteParameter[] parameters)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddRange(parameters);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public static SqliteParameter Parameter(string name, object? value) => new(name, value ?? DBNull.Value);

    private static SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return connection;
    }
}
