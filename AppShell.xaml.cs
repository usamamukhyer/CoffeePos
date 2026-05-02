using DBCafeteria.Views;

namespace DBCafeteria;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(StartupPage), typeof(StartupPage));
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(SignupPage), typeof(SignupPage));
        Routing.RegisterRoute(nameof(GuestDetailsPage), typeof(GuestDetailsPage));
        Routing.RegisterRoute(nameof(GiftDetailsPage), typeof(GiftDetailsPage));
        Routing.RegisterRoute(nameof(ShopByCategoryPage), typeof(ShopByCategoryPage));
        Routing.RegisterRoute(nameof(OurMenuPage), typeof(OurMenuPage));
        Routing.RegisterRoute(nameof(CustomizeCoffeePage), typeof(CustomizeCoffeePage));
        Routing.RegisterRoute(nameof(MyBasketPage), typeof(MyBasketPage));
        Routing.RegisterRoute(nameof(PaymentPage), typeof(PaymentPage));
        Routing.RegisterRoute(nameof(OrderSuccessPage), typeof(OrderSuccessPage));
        Routing.RegisterRoute(nameof(ProfilePage), typeof(ProfilePage));
    }
}
