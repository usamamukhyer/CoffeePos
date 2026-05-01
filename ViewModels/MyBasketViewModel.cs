using System.Collections.ObjectModel;
using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class MyBasketViewModel : BaseViewModel
{
    private readonly OrderSessionService _session = OrderSessionService.Instance;
    private string _errorMessage = string.Empty;

    public ObservableCollection<BasketItemModel> BasketItems => _session.BasketItems;
    public decimal TotalAmount => _session.BasketTotal;
    public string TotalAmountText => AppCurrencySettings.Format(TotalAmount);
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public ICommand IncrementCommand => new RelayCommand(value =>
    {
        if (value is BasketItemModel item)
        {
            item.Quantity++;
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(TotalAmountText));
            OnPropertyChanged(nameof(BasketItems));
        }
    });

    public ICommand DecrementCommand => new RelayCommand(value =>
    {
        if (value is BasketItemModel item && item.Quantity > 1)
        {
            item.Quantity--;
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(TotalAmountText));
            OnPropertyChanged(nameof(BasketItems));
        }
    });

    public ICommand CheckoutCommand => new AsyncCommand(async () =>
    {
        if (BasketItems.Count == 0)
        {
            SetError("Basket is empty.");
            return;
        }

        if (BasketItems.Any(item => item.ProductId <= 0 || item.CategoryId <= 0))
        {
            SetError("Please reload products from the API before checkout.");
            return;
        }

        if (_session.BranchId is null)
        {
            SetError("Please select a branch before checkout.");
            return;
        }

        if (_session.PickupType == PickupType.PreOrder && _session.PickupDateTime is null)
        {
            SetError("Please select a pickup date and time before checkout.");
            return;
        }

        if (_session.OrderType == OrderType.Gift && _session.GiftRecipientCustomerId is null)
        {
            SetError("Please select an existing gift recipient before checkout.");
            return;
        }

        SetError(string.Empty);
        await Shell.Current.GoToAsync(nameof(Views.PaymentPage));
    });

    private void SetError(string message)
    {
        ErrorMessage = message;
        OnPropertyChanged(nameof(HasError));
    }
}
