using System.Windows.Input;

namespace DBCafeteria.ViewModels;

public sealed class ProfileViewModel
{
    public ICommand HomeCommand { get; } = new AsyncCommand(async () => await Shell.Current.GoToAsync("//OrderSetup"));
}
