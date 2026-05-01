using DBCafeteria.Application.DTOs;
using DBCafeteria.Application.Interfaces;
using DBCafeteria.Domain.Entities;
using DBCafeteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DBCafeteria.Infrastructure.Services;

public sealed class AuthService(CafeDbContext db, IPasswordHasher passwordHasher, ITokenService tokenService) : IAuthService
{
    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.EmailOrPhone) || string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Email/phone and password are required.");

        var normalized = request.EmailOrPhone.Trim();
        var phone = long.TryParse(new string(normalized.Where(char.IsDigit).ToArray()), out var parsedPhone) ? parsedPhone : (long?)null;

        var cliente = await db.Clientes.FirstOrDefaultAsync(x =>
            x.Activo && !x.EsInvitado && (x.Email == normalized || (phone != null && x.Telefono == phone)), cancellationToken);

        if (cliente?.PasswordHash is null || !passwordHasher.Verify(request.Password, cliente.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return ToAuthResponse(cliente);
    }

    public async Task<AuthResponse> SignupAsync(SignupRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("Name is required.");
        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Phone))
            throw new InvalidOperationException("Email or phone is required.");
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("Password is required.");

        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim();
        var phone = ParsePhone(request.Phone);

        var exists = await db.Clientes.AnyAsync(x =>
            !x.EsInvitado && ((email != null && x.Email == email) || (phone != null && x.Telefono == phone)), cancellationToken);
        if (exists)
            throw new InvalidOperationException("Customer already exists.");

        var cliente = new Cliente
        {
            Nombre = request.Name.Trim(),
            Email = email,
            Telefono = phone,
            EsInvitado = false,
            PasswordHash = passwordHasher.Hash(request.Password),
            FechaRegistro = DateTime.Now,
            Activo = true
        };

        db.Clientes.Add(cliente);
        await db.SaveChangesAsync(cancellationToken);
        return ToAuthResponse(cliente);
    }

    public async Task<AuthResponse> GuestAsync(GuestRequest request, CancellationToken cancellationToken)
    {
        var cliente = new Cliente
        {
            Nombre = string.IsNullOrWhiteSpace(request.Name) ? "Guest" : request.Name.Trim(),
            Telefono = ParsePhone(request.Phone),
            EsInvitado = true,
            FechaRegistro = DateTime.Now,
            Activo = true
        };

        db.Clientes.Add(cliente);
        await db.SaveChangesAsync(cancellationToken);
        return ToAuthResponse(cliente);
    }

    private AuthResponse ToAuthResponse(Cliente cliente) =>
        new(cliente.Id, cliente.Nombre ?? "Guest", cliente.Email, cliente.Telefono?.ToString(), cliente.EsInvitado, tokenService.CreateAccessToken(cliente), tokenService.CreateRefreshToken());

    private static long? ParsePhone(string? phone)
    {
        var digits = new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());
        return long.TryParse(digits, out var value) ? value : null;
    }
}
