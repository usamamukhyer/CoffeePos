namespace DBCafeteria.Views;

public partial class StartupPage : ContentPage
{
    private bool _animated;

    public StartupPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_animated)
            return;

        _animated = true;
        await Task.WhenAll(
            CupMark.ScaleToAsync(1.08, 650, Easing.CubicInOut),
            CupMark.FadeToAsync(0.88, 650, Easing.CubicInOut));
        await Task.WhenAll(
            CupMark.ScaleToAsync(1, 650, Easing.CubicInOut),
            CupMark.FadeToAsync(1, 650, Easing.CubicInOut));
    }
}
