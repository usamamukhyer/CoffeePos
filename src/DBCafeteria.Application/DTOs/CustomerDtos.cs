namespace DBCafeteria.Application.DTOs;

public sealed record CustomerSearchResultDto(int Id, string Name, string Phone, string? Email);
