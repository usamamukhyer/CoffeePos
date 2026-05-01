using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

public partial class MyBasketPage : ContentPage
{
    public MyBasketPage()
    {
        InitializeComponent();
        BindingContext = new MyBasketViewModel();
    }
}
