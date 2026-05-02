using System.Collections.ObjectModel;
using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class OurMenuViewModel : BaseViewModel
{
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
    private readonly LocalMenuCacheService _cache = new();
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public ObservableCollection<ProductModel> Products { get; } = new();
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public bool IsEmpty => !IsBusy && !HasError && Products.Count == 0;

    public ICommand BackCommand { get; } = new AsyncCommand(async () => await Shell.Current.GoToAsync(".."));
    public ICommand SelectProductCommand => new RelayCommand(async value =>
    {
        if (value is ProductModel product)
        {
            OrderSessionService.Instance.SelectedProduct = product;
            await Shell.Current.GoToAsync($"{nameof(Views.CustomizeCoffeePage)}?productName={Uri.EscapeDataString(product.Name)}");
        }
    });

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            OnPropertyChanged(nameof(HasError));

            var category = OrderSessionService.Instance.SelectedCategory;
            if (category?.Id is null or <= 0)
            {
                Products.Clear();
                ErrorMessage = "Selecciona una categoria primero.";
                OnPropertyChanged(nameof(HasError));
                OnPropertyChanged(nameof(IsEmpty));
                return;
            }

            var cachedProducts = await _cache.GetProductsAsync(category.Id);
            ReplaceProducts(cachedProducts);
            _ = SyncProductsAsync(category.Id);
            if (cachedProducts.Count > 0)
                return;

            var products = await _apiClient.GetAsync<IReadOnlyList<ApiProduct>>($"api/categories/{category.Id}/subcategories") ?? [];
            await _cache.UpsertProductsAsync(products);
            ReplaceProducts(await _cache.GetProductsAsync(category.Id));
            OnPropertyChanged(nameof(IsEmpty));
        }
        catch (Exception ex) when (ex is HttpRequestException or ApiException)
        {
            ErrorMessage = "No se pudo cargar el menu. Intentalo de nuevo.";
            OnPropertyChanged(nameof(HasError));

            Products.Clear();
            OnPropertyChanged(nameof(IsEmpty));
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    private async Task SyncProductsAsync(int categoryId)
    {
        try
        {
            var products = await _apiClient.GetAsync<IReadOnlyList<ApiProduct>>($"api/categories/{categoryId}/subcategories") ?? [];
            await _cache.UpsertProductsAsync(products);
        }
        catch (Exception ex) when (ex is HttpRequestException or ApiException)
        {
        }
    }

    private void ReplaceProducts(IEnumerable<ProductModel> products)
    {
        Products.Clear();
        foreach (var product in products)
            Products.Add(product);
        OnPropertyChanged(nameof(IsEmpty));
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
