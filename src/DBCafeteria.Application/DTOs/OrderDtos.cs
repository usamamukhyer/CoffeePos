namespace DBCafeteria.Application.DTOs;

public sealed record CreateOrderRequest(
    int? CustomerId,
    bool IsGuest,
    int BranchId,
    string OrderType,
    string PickupType,
    DateTime? PickupDateTime,
    IReadOnlyList<CreateOrderItemRequest> Items,
    PaymentRequest? Payment,
    GiftRequest? Gift);

public sealed record CreateOrderItemRequest(
    int CategoryId,
    int ProductId,
    int Quantity,
    int? DrinkType,
    int? CupId,
    int? MilkId,
    int? BeanTypeId,
    double UnitPrice,
    double TaxRate,
    IReadOnlyList<CreateOrderToppingRequest> Toppings);

public sealed record CreateOrderToppingRequest(int ToppingId, int Quantity, double UnitPrice, double TaxRate);
public sealed record PaymentRequest(string PaymentMethod, string Currency, double Amount, double ExchangeRate, int UserId);
public sealed record GiftRequest(int RecipientCustomerId, string RecipientName, string RecipientPhone, string? Message);
public sealed record CreateOrderResponse(int OrderId, int OrderNumber, double Total, string? GiftCode);
