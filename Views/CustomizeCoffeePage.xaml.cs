using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

[QueryProperty(nameof(ProductName), "productName")]
public partial class CustomizeCoffeePage : ContentPage
{
    private readonly CustomizeCoffeeViewModel _viewModel = new();

    public CustomizeCoffeePage()
    {
        InitializeComponent();
        BindingContext = _viewModel;
    }

    public string ProductName
    {
        set => _viewModel.Initialize(Uri.UnescapeDataString(value ?? string.Empty));
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadAsync();
    }
}
