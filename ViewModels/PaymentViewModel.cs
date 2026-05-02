using System.Windows.Input;
using DBCafeteria.Models;
using DBCafeteria.Services;
using Microsoft.Maui.Networking;

namespace DBCafeteria.ViewModels;

public sealed class PaymentViewModel : BaseViewModel
{
    private const string CashOnDelivery = "Pago contra entrega";
    private const string OfflineOrderMessage = "Gracias. Tu pedido fue recibido y se procesara automaticamente cuando vuelva la conexion.";
    private const string ServerDownMessage = "Gracias. Tu pedido fue recibido y se procesara automaticamente cuando el sistema este disponible.";

    private readonly OrderSessionService _session = OrderSessionService.Instance;
    private readonly CafeApiClient _apiClient = ApiClientFactory.Create();
    private readonly SyncQueueService _syncQueue = new();
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
            var clientOrderId = request.ClientOrderId!;
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await SavePendingOrderAsync(clientOrderId, request, "PendingSync", OfflineOrderMessage);
                return;
            }

            var response = await _apiClient.PostAsync<ApiOrderRequest, ApiOrderResponse>("orders/place-order", request);
            if (response is null)
            {
                SetError("No se pudo completar el pedido. Intentalo de nuevo.");
                return;
            }

            await _syncQueue.EnqueueOrderAsync(clientOrderId, request, "Synced");
            _session.BasketItems.Clear();
            _session.CurrentClientOrderId = Guid.NewGuid().ToString("N");
            await Shell.Current.GoToAsync(nameof(Views.OrderSuccessPage));
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode < 500)
            {
                SetError(ex.Message);
                return;
            }

            var request = BuildOrderRequest();
            await SavePendingOrderAsync(request.ClientOrderId!, request, "PendingServer", ServerDownMessage);
        }
        catch (HttpRequestException)
        {
            var request = BuildOrderRequest();
            await SavePendingOrderAsync(request.ClientOrderId!, request, "PendingServer", ServerDownMessage);
        }
        catch (TaskCanceledException)
        {
            var request = BuildOrderRequest();
            await SavePendingOrderAsync(request.ClientOrderId!, request, "PendingServer", ServerDownMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool ValidateCheckout()
    {
        if (_session.BasketItems.Count == 0)
            return Fail("La canasta esta vacia.");

        if (_session.BasketItems.Any(item => item.ProductId <= 0 || item.CategoryId <= 0))
            return Fail("Recarga los productos antes de finalizar el pedido.");

        if (_session.BranchId is null)
            return Fail("Selecciona una sucursal antes de finalizar el pedido.");

        if (_session.PickupType == PickupType.PreOrder && _session.PickupDateTime is null)
            return Fail("Selecciona fecha y hora de recoleccion antes de finalizar el pedido.");

        if (_session.OrderType == OrderType.Gift && _session.GiftRecipientCustomerId is null)
            return Fail("Selecciona un destinatario existente para el regalo.");

        return true;
    }

    private ApiOrderRequest BuildOrderRequest() =>
        new(
            _session.CurrentClientOrderId,
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

    private async Task SavePendingOrderAsync(string clientOrderId, ApiOrderRequest request, string status, string message)
    {
        await _syncQueue.EnqueueOrderAsync(clientOrderId, request, status, message);
        _ = OfflineSyncService.Instance.TriggerSyncAsync();
        _session.BasketItems.Clear();
        _session.CurrentClientOrderId = Guid.NewGuid().ToString("N");
        await Shell.Current.DisplayAlertAsync("Pedido recibido", message, "OK");
        await Shell.Current.GoToAsync(nameof(Views.OrderSuccessPage));
    }

    private static ApiOrderItemRequest BuildOrderItem(BasketItemModel item) =>
        new(
            item.CategoryId,
            item.ProductId,
            item.Quantity,
            IsCold(item.Temperature) ? 2 : 1,
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

    private static bool IsCold(string temperature) =>
        temperature.Equals("Cold", StringComparison.OrdinalIgnoreCase) ||
        temperature.Equals("Frio", StringComparison.OrdinalIgnoreCase);
}
