using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

public partial class GiftDetailsPage : ContentPage
{
    public GiftDetailsPage()
    {
        InitializeComponent();
        BindingContext = new GiftDetailsViewModel();
    }
}
