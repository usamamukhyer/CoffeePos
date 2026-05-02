using System.Collections.ObjectModel;
using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class CustomizeCoffeeViewModel : BaseViewModel
{
    private readonly OrderSessionService _session = OrderSessionService.Instance;
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
    private readonly LocalMenuCacheService _cache = new();
    private string _productName = string.Empty;
    private string _temperature = string.Empty;
    private string _bean = string.Empty;
    private string _size = string.Empty;
    private decimal _basePrice;
    private decimal _totalAmount;
    private string _errorMessage = string.Empty;
    private bool _isBusy;
    private bool _hasLoaded;

    public string ProductName { get => _productName; set => SetProperty(ref _productName, value); }
    public string Temperature { get => _temperature; set { if (SetProperty(ref _temperature, value)) RefreshSelections(); } }
    public string Bean { get => _bean; set { if (SetProperty(ref _bean, value)) RefreshSelections(); } }
    public string Size { get => _size; set { if (SetProperty(ref _size, value)) RefreshSelections(); } }
    public decimal TotalAmount { get => _totalAmount; set { if (SetProperty(ref _totalAmount, value)) OnPropertyChanged(nameof(TotalAmountText)); } }
    public string TotalAmountText => AppCurrencySettings.Format(TotalAmount);
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }
    public bool IsEmpty => !IsBusy && !HasError && !_hasLoaded;

    public ObservableCollection<ChoiceViewModel> Temperatures { get; } = [];
    public ObservableCollection<ChoiceViewModel> Beans { get; } = [];
    public ObservableCollection<ChoiceViewModel> Sizes { get; } = [];
    public ObservableCollection<ChoiceViewModel> MilkOptions { get; } = [];
    public ObservableCollection<ChoiceViewModel> Toppings { get; } = [];

    public ICommand SelectTemperatureCommand => new RelayCommand(value => Temperature = value?.ToString() ?? string.Empty);
    public ICommand SelectBeanCommand => new RelayCommand(value => Bean = value?.ToString() ?? string.Empty);
    public ICommand SelectSizeCommand => new RelayCommand(value => Size = value?.ToString() ?? string.Empty);
    public ICommand SelectMilkCommand => new RelayCommand(value =>
    {
        foreach (var item in MilkOptions)
            item.IsSelected = item.Title == value?.ToString();
        CalculateTotal();
    });
    public ICommand IncrementToppingCommand => new RelayCommand(value =>
    {
        if (value is ChoiceViewModel topping)
        {
            topping.Quantity++;
            CalculateTotal();
        }
    });
    public ICommand DecrementToppingCommand => new RelayCommand(value =>
    {
        if (value is ChoiceViewModel topping && topping.Quantity > 0)
        {
            topping.Quantity--;
            CalculateTotal();
        }
    });
    public ICommand AddToBasketCommand => new AsyncCommand(AddToBasketAsync);
    public ICommand BackCommand { get; } = new AsyncCommand(async () => await Shell.Current.GoToAsync(".."));

    public async Task LoadAsync()
    {
        if (IsBusy || _hasLoaded)
            return;

        var selectedProduct = _session.SelectedProduct;
        if (selectedProduct?.Id is null or <= 0)
        {
            SetError("No se pudo cargar el menu. Intentalo de nuevo.");
            return;
        }

        try
        {
            IsBusy = true;
            SetError(string.Empty);

            var customization = await _cache.GetCustomizationAsync(selectedProduct.Id);
            _ = OfflineSyncService.Instance.SyncCustomizationAsync(selectedProduct.Id);
            customization ??= await _apiClient.GetAsync<ApiCustomization>($"api/menu/customization/{selectedProduct.Id}");
            if (customization is null)
            {
                SetError("No se pudo cargar el menu. Intentalo de nuevo.");
                return;
            }

            await _cache.UpsertCustomizationAsync(selectedProduct.Id, customization);

            selectedProduct.Name = customization.Product.Name;
            selectedProduct.BasePrice = (decimal)(customization.Product.PriceWithTax ?? customization.Product.Price ?? 0);
            selectedProduct.HotAvailable = customization.Product.HotAvailable;
            selectedProduct.ColdAvailable = customization.Product.ColdAvailable;
            ProductName = customization.Product.Name;
            _basePrice = selectedProduct.BasePrice;

            ReplaceChoices(Temperatures, customization.Temperatures.Select(x => new ChoiceViewModel(ToDisplayTemperature(x))));
            ReplaceChoices(Beans, customization.BeanTypes.Select(ToChoice));
            ReplaceChoices(Sizes, customization.Sizes.Select(ToChoice));
            ReplaceChoices(MilkOptions, customization.Milks.Select(ToChoice));
            ReplaceChoices(Toppings, customization.Toppings.Select(ToChoice));

            Temperature = Temperatures.FirstOrDefault()?.Title ?? string.Empty;
            Bean = Beans.FirstOrDefault()?.Title ?? string.Empty;
            Size = Sizes.FirstOrDefault()?.Title ?? string.Empty;
            if (MilkOptions.FirstOrDefault() is { } firstMilk)
                firstMilk.IsSelected = true;

            _hasLoaded = true;
            RefreshSelections();
            CalculateTotal();
        }
        catch (Exception ex) when (ex is HttpRequestException or ApiException)
        {
            SetError("No se pudo cargar el menu. Intentalo de nuevo.");
            ClearCustomization();
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(IsEmpty));
        }
    }

    public void Initialize(string productName)
    {
        if (!string.IsNullOrWhiteSpace(productName))
            ProductName = productName;
    }

    private async Task AddToBasketAsync()
    {
        var selectedProduct = _session.SelectedProduct;
        if (selectedProduct?.Id is null or <= 0 || !_hasLoaded)
        {
            SetError("No se pudo cargar el menu. Intentalo de nuevo.");
            return;
        }

        var selectedMilk = MilkOptions.FirstOrDefault(item => item.IsSelected);
        var selectedSize = Sizes.FirstOrDefault(item => item.IsSelected);
        var selectedBean = Beans.FirstOrDefault(item => item.IsSelected);

        _session.BasketItems.Add(new BasketItemModel
        {
            ProductId = selectedProduct.Id,
            CategoryId = selectedProduct.CategoryId,
            CupId = selectedSize?.Id,
            MilkId = selectedMilk?.Id,
            BeanTypeId = selectedBean?.Id,
            Name = ProductName,
            Temperature = Temperature,
            Size = selectedSize?.Title ?? string.Empty,
            Milk = selectedMilk?.Title ?? string.Empty,
            Price = TotalAmount,
            Quantity = 1,
            Toppings = Toppings
                .Where(item => item.Quantity > 0)
                .Select(item => new BasketToppingModel
                {
                    ToppingId = item.Id,
                    Name = item.Title,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    TaxRate = 0
                })
                .ToList()
        });

        await Shell.Current.GoToAsync(nameof(Views.MyBasketPage));
    }

    private static ChoiceViewModel ToChoice(ApiOption option)
    {
        var price = (decimal)(option.PriceWithTax != 0 ? option.PriceWithTax : option.Price);
        return new ChoiceViewModel(option.Name, AppCurrencySettings.Format(price), price, option.Id);
    }

    private static void ReplaceChoices(ObservableCollection<ChoiceViewModel> target, IEnumerable<ChoiceViewModel> source)
    {
        target.Clear();
        foreach (var item in source)
            target.Add(item);
    }

    private void RefreshSelections()
    {
        foreach (var item in Temperatures) item.IsSelected = item.Title == Temperature;
        foreach (var item in Beans) item.IsSelected = item.Title == Bean;
        foreach (var item in Sizes) item.IsSelected = item.Title == Size;
        CalculateTotal();
    }

    private void CalculateTotal()
    {
        TotalAmount = _basePrice
            + Sizes.Where(item => item.IsSelected).Sum(item => item.Price)
            + MilkOptions.Where(item => item.IsSelected).Sum(item => item.Price)
            + Beans.Where(item => item.IsSelected).Sum(item => item.Price)
            + Toppings.Sum(item => item.Price * item.Quantity);
    }

    private void ClearCustomization()
    {
        Temperatures.Clear();
        Beans.Clear();
        Sizes.Clear();
        MilkOptions.Clear();
        Toppings.Clear();
        _hasLoaded = false;
        OnPropertyChanged(nameof(IsEmpty));
    }

    private void SetError(string message)
    {
        ErrorMessage = message;
        OnPropertyChanged(nameof(HasError));
    }

    private static string ToDisplayTemperature(string value) =>
        value.Equals("Hot", StringComparison.OrdinalIgnoreCase) ? "Caliente" :
        value.Equals("Cold", StringComparison.OrdinalIgnoreCase) ? "Frio" :
        value;

}
