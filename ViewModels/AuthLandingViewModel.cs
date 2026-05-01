using System.Windows.Input;

namespace DBCafeteria.ViewModels;

public sealed class AuthLandingViewModel
{
    public ICommand LoginCommand { get; } = new AsyncCommand(async () => await Shell.Current.GoToAsync(nameof(Views.LoginPage)));
    public ICommand SignupCommand { get; } = new AsyncCommand(async () => await Shell.Current.GoToAsync(nameof(Views.SignupPage)));
    public ICommand GuestCommand { get; } = new AsyncCommand(async () => await Shell.Current.GoToAsync(nameof(Views.GuestDetailsPage)));
}
