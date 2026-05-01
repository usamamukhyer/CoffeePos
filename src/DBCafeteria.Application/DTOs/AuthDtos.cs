namespace DBCafeteria.Application.DTOs;

public sealed record LoginRequest(string EmailOrPhone, string Password);
public sealed record SignupRequest(string Name, string? Email, string? Phone, string Password);
public sealed record GuestRequest(string? Name, string? Phone);
public sealed record AuthResponse(int CustomerId, string CustomerName, string? Email, string? Phone, bool IsGuest, string AccessToken, string RefreshToken);
