using DBCafeteria.Application.DTOs;

namespace DBCafeteria.Application.Interfaces;

public interface IOrderService
{
    CartValidationResponse ValidateCart(CartValidationRequest request);
    Task<CreateOrderResponse> CreateOrderAsync(CreateOrderRequest request, CancellationToken cancellationToken);
}
