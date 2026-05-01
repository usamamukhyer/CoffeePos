using DBCafeteria.ViewModels;

namespace DBCafeteria.Views;

public partial class OrderSetupPage : ContentPage
{
    private readonly OrderSetupViewModel _viewModel;

    public OrderSetupPage()
    {
        InitializeComponent();
        _viewModel = new OrderSetupViewModel();
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Branches.Count == 0 && _viewModel.LoadBranchesCommand.CanExecute(null))
            _viewModel.LoadBranchesCommand.Execute(null);
    }
}
