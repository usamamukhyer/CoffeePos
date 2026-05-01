using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;

namespace DBCafeteria.ViewModels;

public sealed class PaymentViewModel : BaseViewModel
{
    private const string CashOnDelivery = "Cash on Delivery";

    private readonly OrderSessionService _session = OrderSessionService.Instance;
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
    private string _errorMessage = string.Empty;
    private bool _isBusy;

    public decimal ItemsTotal => _session.BasketTotal;
    public decimal Tax => 0;
    public decimal GrandTotal => ItemsTotal + Tax;
    public string ItemsTotalText => AppCurrencySettings.Format(ItemsTotal);
    public string TaxText => AppCurrencySettings.Format(Tax);
    public string GrandTotalText => AppCurrencySettings.Format(GrandTotal);
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }
    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
    public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

    public ICommand PlaceOrderCommand => new AsyncCommand(PlaceOrderAsync);

    private async Task PlaceOrderAsync()
    {
        if (!ValidateCheckout())
            return;

        try
        {
            IsBusy = true;
            SetError(string.Empty);

            var request = BuildOrderRequest();
            var response = await _apiClient.PostAsync<ApiOrderRequest, ApiOrderResponse>("orders/place-order", request);
            if (response is null)
            {
                SetError("Order failed. Please try again.");
                return;
            }

            _session.BasketItems.Clear();
            await Shell.Current.GoToAsync(nameof(Views.OrderSuccessPage));
        }
        catch (ApiException ex)
        {
            SetError(ex.Message);
        }
        catch (HttpRequestException)
        {
            SetError("API is not reachable. Please run the API on http://localhost:5126.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateCheckout()
    {
        if (_session.BasketItems.Count == 0)
            return Fail("Basket is empty.");

        if (_session.BasketItems.Any(item => item.ProductId <= 0 || item.CategoryId <= 0))
            return Fail("Please reload products from the API before checkout.");

        if (_session.BranchId is null)
            return Fail("Please select a branch before checkout.");

        if (_session.PickupType == PickupType.PreOrder && _session.PickupDateTime is null)
            return Fail("Please select a pickup date and time before checkout.");

        if (_session.OrderType == OrderType.Gift && _session.GiftRecipientCustomerId is null)
            return Fail("Please select an existing gift recipient before checkout.");

        return true;
    }

    private ApiOrderRequest BuildOrderRequest() =>
        new(
            _session.CustomerId,
            _session.IsGuest,
            _session.BranchId!.Value,
            _session.OrderType == OrderType.Gift ? "Gift" : "ForMe",
            _session.PickupType == PickupType.PreOrder ? "PreOrder" : "Now",
            _session.PickupType == PickupType.PreOrder ? _session.PickupDateTime : null,
            _session.BasketItems.Select(BuildOrderItem).ToList(),
            new ApiPaymentRequest(CashOnDelivery, AppCurrencySettings.DefaultCurrencyCode, (double)GrandTotal, 1, 0),
            _session.OrderType == OrderType.Gift
                ? new ApiGiftRequest(_session.GiftRecipientCustomerId!.Value, _session.GiftRecipientName, _session.GiftRecipientPhone, _session.GiftMessage)
                : null);

    private static ApiOrderItemRequest BuildOrderItem(BasketItemModel item) =>
        new(
            item.CategoryId,
            item.ProductId,
            item.Quantity,
            item.Temperature.Equals("Cold", StringComparison.OrdinalIgnoreCase) ? 2 : 1,
            item.CupId,
            item.MilkId,
            item.BeanTypeId,
            (double)item.Price,
            0,
            item.Toppings.Select(topping => new ApiOrderToppingRequest(
                topping.ToppingId,
                topping.Quantity,
                (double)topping.UnitPrice,
                topping.TaxRate)).ToList());

    private bool Fail(string message)
    {
        SetError(message);
        return false;
    }

    private void SetError(string message)
    {
        ErrorMessage = message;
        OnPropertyChanged(nameof(HasError));
    }
}
