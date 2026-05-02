namespace DBCafeteria.Application.DTOs;

public sealed record CustomerSearchResultDto(int Id, string Name, string Phone, string? Email);
public sealed record CustomerSyncItemDto(int Id, string Name, string Phone, string? Email, bool IsGuest, bool Active, string Version);
