using DBCafeteria.Application.DTOs;

namespace DBCafeteria.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> SignupAsync(SignupRequest request, CancellationToken cancellationToken);
    Task<AuthResponse> GuestAsync(GuestRequest request, CancellationToken cancellationToken);
}
