using System.Windows.Input;

namespace DBCafeteria.ViewModels;

public sealed class OrderSuccessViewModel
{
    public ICommand BackHomeCommand { get; } = new AsyncCommand(async () => await Shell.Current.GoToAsync("//OrderSetup"));
}
