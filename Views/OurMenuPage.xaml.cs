using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

public partial class OurMenuPage : ContentPage
{
    public OurMenuPage()
    {
        InitializeComponent();
        BindingContext = new OurMenuViewModel();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is OurMenuViewModel viewModel)
            await viewModel.LoadAsync();
    }
}
