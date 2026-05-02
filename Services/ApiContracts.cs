namespace DBCafeteria.Services;

public sealed record ApiAuthResponse(int CustomerId, string CustomerName, string? Email, string? Phone, bool IsGuest, string AccessToken, string RefreshToken);
public sealed record ApiLoginRequest(string EmailOrPhone, string Password);
public sealed record ApiSignupRequest(string Name, string? Email, string? Phone, string Password);
public sealed record ApiGuestRequest(string? Name, string? Phone);
public sealed record ApiCategory(int Id, string Name, string Key, double? Price, double? PriceWithTax);
public sealed record ApiProduct(int Id, int CategoryId, string Name, bool HotAvailable, bool ColdAvailable, double? Price, double? PriceWithTax);
public sealed record ApiBranch(int Id, string Name, string? Address);
public sealed record ApiCustomerSearchResult(int Id, string Name, string Phone, string? Email);
public sealed record ApiCustomerSyncItem(int Id, string Name, string Phone, string? Email, bool IsGuest, bool Active, string Version);
public sealed record ApiCurrency(string Code, string Symbol);
public sealed record ApiOption(int Id, string Name, double Price, double PriceWithTax);
public sealed record ApiCustomization(
    ApiProduct Product,
    ApiCurrency Currency,
    IReadOnlyList<string> Temperatures,
    IReadOnlyList<ApiOption> BeanTypes,
    IReadOnlyList<ApiOption> Sizes,
    IReadOnlyList<ApiOption> Milks,
    IReadOnlyList<ApiOption> Toppings);
public sealed record ApiOrderRequest(
    string? ClientOrderId,
    int? CustomerId,
    bool IsGuest,
    int BranchId,
    string OrderType,
    string PickupType,
    DateTime? PickupDateTime,
    IReadOnlyList<ApiOrderItemRequest> Items,
    ApiPaymentRequest? Payment,
    ApiGiftRequest? Gift);
public sealed record ApiOrderItemRequest(
    int CategoryId,
    int ProductId,
    int Quantity,
    int? DrinkType,
    int? CupId,
    int? MilkId,
    int? BeanTypeId,
    double UnitPrice,
    double TaxRate,
    IReadOnlyList<ApiOrderToppingRequest> Toppings);
public sealed record ApiOrderToppingRequest(int ToppingId, int Quantity, double UnitPrice, double TaxRate);
public sealed record ApiPaymentRequest(string PaymentMethod, string Currency, double Amount, double ExchangeRate, int UserId);
public sealed record ApiGiftRequest(int RecipientCustomerId, string RecipientName, string RecipientPhone, string? Message);
public sealed record ApiOrderResponse(int OrderId, int OrderNumber, double Total, string? GiftCode);
