using System.Collections.ObjectModel;
using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class OurMenuViewModel : BaseViewModel
{
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
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
                ErrorMessage = "Please select a category first.";
                OnPropertyChanged(nameof(HasError));
                OnPropertyChanged(nameof(IsEmpty));
                return;
            }

            var route = $"api/categories/{category.Id}/subcategories";
            var products = await _apiClient.GetAsync<IReadOnlyList<ApiProduct>>(route) ?? [];

            Products.Clear();
            foreach (var product in products)
            {
                Products.Add(new ProductModel
                {
                    Id = product.Id,
                    CategoryId = product.CategoryId,
                    Name = product.Name,
                    BasePrice = (decimal)(product.PriceWithTax ?? product.Price ?? 0),
                    HotAvailable = product.HotAvailable,
                    ColdAvailable = product.ColdAvailable,
                    Image = GetProductImage(product.Name)
                });
            }
            OnPropertyChanged(nameof(IsEmpty));
        }
        catch (Exception ex) when (ex is HttpRequestException or ApiException)
        {
            ErrorMessage = "Unable to load menu. Please try again.";
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
