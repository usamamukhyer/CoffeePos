using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

public partial class PaymentPage : ContentPage
{
    public PaymentPage()
    {
        InitializeComponent();
        BindingContext = new PaymentViewModel();
    }
}
