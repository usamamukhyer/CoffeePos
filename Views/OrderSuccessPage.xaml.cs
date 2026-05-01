using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

public partial class OrderSuccessPage : ContentPage
{
    public OrderSuccessPage()
    {
        InitializeComponent();
        BindingContext = new OrderSuccessViewModel();
    }
}
