namespace DBCafeteria.Application.DTOs;

public sealed record GiftDto(int Id, int OrderId, string RecipientName, string RecipientPhone, string? Message, string? GiftCode, string Status, DateTime CreatedAt);
