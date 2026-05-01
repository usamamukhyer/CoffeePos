using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

public partial class AuthLandingPage : ContentPage
{
    public AuthLandingPage()
    {
        InitializeComponent();
        BindingContext = new AuthLandingViewModel();
    }
}
