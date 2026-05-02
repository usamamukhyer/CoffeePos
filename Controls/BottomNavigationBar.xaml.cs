using DBCafeteria.Services;

namespace DBCafeteria.Controls;

public partial class BottomNavigationBar : ContentView
{
    private static readonly Color Active = Color.FromArgb("#1D2E45");
    private static readonly Color Inactive = Color.FromArgb("#555555");

    public static readonly BindableProperty ActiveTabProperty = BindableProperty.Create(nameof(ActiveTab), typeof(string), typeof(BottomNavigationBar), "Order", propertyChanged: OnActiveChanged);

    public BottomNavigationBar()
    {
        InitializeComponent();
        Refresh();
    }

    public string ActiveTab { get => (string)GetValue(ActiveTabProperty); set => SetValue(ActiveTabProperty, value); }
    public string CartText => OrderSessionService.Instance.BasketCount > 0 ? $"Canasta ({OrderSessionService.Instance.BasketCount})" : "Canasta";
    public Color OrderColor => ActiveTab == "Order" ? Active : Inactive;
    public Color MenuColor => ActiveTab == "Menu" ? Active : Inactive;
    public Color CartColor => ActiveTab == "Cart" ? Active : Inactive;
    public Color ProfileColor => ActiveTab == "Profile" ? Active : Inactive;
    public FontAttributes MenuFont => ActiveTab == "Menu" ? FontAttributes.Bold : FontAttributes.None;
    public FontAttributes CartFont => ActiveTab == "Cart" ? FontAttributes.Bold : FontAttributes.None;

    protected override void OnParentSet()
    {
        base.OnParentSet();
        Refresh();
    }

    private static void OnActiveChanged(BindableObject bindable, object oldValue, object newValue) => ((BottomNavigationBar)bindable).Refresh();

    private void Refresh()
    {
        OnPropertyChanged(nameof(CartText));
        OnPropertyChanged(nameof(OrderColor));
        OnPropertyChanged(nameof(MenuColor));
        OnPropertyChanged(nameof(CartColor));
        OnPropertyChanged(nameof(ProfileColor));
        OnPropertyChanged(nameof(MenuFont));
        OnPropertyChanged(nameof(CartFont));
    }

    private async void OnOrderTapped(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync("//OrderSetup");
    private async void OnMenuTapped(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync(nameof(Views.ShopByCategoryPage));
    private async void OnCartTapped(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync(nameof(Views.MyBasketPage));
    private async void OnProfileTapped(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync(nameof(Views.ProfilePage));
}
