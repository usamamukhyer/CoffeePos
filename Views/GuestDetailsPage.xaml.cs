using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

public partial class GuestDetailsPage : ContentPage
{
    public GuestDetailsPage()
    {
        InitializeComponent();
        BindingContext = new GuestDetailsViewModel();
    }
}
