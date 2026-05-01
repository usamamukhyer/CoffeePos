using DBCafeteria.Application.DTOs;

namespace DBCafeteria.Application.Interfaces;

public interface IGiftService
{
    Task<GiftDto> CreateGiftAsync(int orderId, GiftRequest request, int? senderCustomerId, CancellationToken cancellationToken);
    Task<GiftDto?> GetGiftAsync(int id, CancellationToken cancellationToken);
}
