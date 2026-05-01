namespace DBCafeteria.Application.DTOs;

public sealed record CartValidationRequest(IReadOnlyList<CreateOrderItemRequest> Items);
public sealed record CartValidationResponse(bool IsValid, double Subtotal, double Tax, double Total, IReadOnlyList<string> Messages);
