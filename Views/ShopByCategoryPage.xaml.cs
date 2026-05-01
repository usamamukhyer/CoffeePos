using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

public partial class ShopByCategoryPage : ContentPage
{
    public ShopByCategoryPage()
    {
        InitializeComponent();
        BindingContext = new ShopByCategoryViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is ShopByCategoryViewModel viewModel)
            await viewModel.LoadAsync();
    }
}
