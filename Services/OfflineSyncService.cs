using Microsoft.Maui.Networking;

namespace DBCafeteria.Services;

public sealed class OfflineSyncService : IDisposable
{
    public static OfflineSyncService Instance { get; } = new();

    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
    private readonly LocalMenuCacheService _menuCache = new();
    private readonly LocalBranchCacheService _branchCache = new();
    private readonly LocalCustomerCacheService _customerCache = new();
    private readonly SyncQueueService _syncQueue = new();
    private readonly PeriodicTimer _timer = new(TimeSpan.FromSeconds(30));
    private readonly SemaphoreSlim _syncGate = new(1, 1);
    private CancellationTokenSource _cts = new();
    private bool _started;
    private bool _isActive;

    private OfflineSyncService()
    {
    }

    public void Start()
    {
        if (_started)
            return;

        _started = true;
        _isActive = true;
        Connectivity.Current.ConnectivityChanged += OnConnectivityChanged;
        _ = LocalCacheDatabase.Instance.InitializeAsync();
        _ = TriggerSyncAsync();
        _ = RunTimerAsync(_cts.Token);
    }

    public void MarkActive()
    {
        _isActive = true;
        _ = TriggerSyncAsync();
    }

    public void MarkInactive() => _isActive = false;

    public async Task TriggerSyncAsync(CancellationToken cancellationToken = default)
    {
        if (!_isActive || Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return;

        if (!await _syncGate.WaitAsync(0, cancellationToken))
            return;

        try
        {
            await _syncQueue.FlushPendingOrdersAsync(cancellationToken);
            await TrySyncReferenceDataAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or ApiException)
        {
            // Silent by design: UI renders from SQLite and pending orders remain queued.
        }
        finally
        {
            _syncGate.Release();
        }
    }

    private async Task SyncReferenceDataAsync(CancellationToken cancellationToken)
    {
        var categories = await _apiClient.GetAsync<IReadOnlyList<ApiCategory>>("api/categories", cancellationToken) ?? [];
        await _menuCache.UpsertCategoriesAsync(categories, cancellationToken);

        foreach (var category in categories)
        {
            var products = await _apiClient.GetAsync<IReadOnlyList<ApiProduct>>($"api/categories/{category.Id}/subcategories", cancellationToken) ?? [];
            await _menuCache.UpsertProductsAsync(products, cancellationToken);
        }

        await _menuCache.UpsertOptionsAsync("Milk", await _apiClient.GetAsync<IReadOnlyList<ApiOption>>("menu/milks", cancellationToken) ?? [], cancellationToken);
        await _menuCache.UpsertOptionsAsync("Beans", await _apiClient.GetAsync<IReadOnlyList<ApiOption>>("menu/beans", cancellationToken) ?? [], cancellationToken);
        await _menuCache.UpsertOptionsAsync("Toppings", await _apiClient.GetAsync<IReadOnlyList<ApiOption>>("menu/toppings", cancellationToken) ?? [], cancellationToken);
        await _menuCache.UpsertOptionsAsync("Sizes", await _apiClient.GetAsync<IReadOnlyList<ApiOption>>("menu/cups", cancellationToken) ?? [], cancellationToken);

        var branches = await _apiClient.GetAsync<IReadOnlyList<ApiBranch>>("api/branches", cancellationToken) ?? [];
        await _branchCache.UpsertBranchesAsync(branches, cancellationToken);

        var customers = await _apiClient.GetAsync<IReadOnlyList<ApiCustomerSyncItem>>("api/customers/sync", cancellationToken) ?? [];
        await _customerCache.UpsertCustomersAsync(customers, cancellationToken);
    }

    private async Task TrySyncReferenceDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            await SyncReferenceDataAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or ApiException)
        {
            // Reference data can retry later; it should not stop queued orders from reaching the API.
        }
    }

    public async Task SyncCustomizationAsync(int productId, CancellationToken cancellationToken = default)
    {
        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            return;

        try
        {
            var customization = await _apiClient.GetAsync<ApiCustomization>($"api/menu/customization/{productId}", cancellationToken);
            if (customization is not null)
                await _menuCache.UpsertCustomizationAsync(productId, customization, cancellationToken);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or ApiException)
        {
        }
    }

    private async Task RunTimerAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (await _timer.WaitForNextTickAsync(cancellationToken))
                await TriggerSyncAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        if (_isActive && e.NetworkAccess == NetworkAccess.Internet)
            _ = TriggerSyncAsync();
    }

    public void Dispose()
    {
        Connectivity.Current.ConnectivityChanged -= OnConnectivityChanged;
        _cts.Cancel();
        _timer.Dispose();
        _cts.Dispose();
    }
}
