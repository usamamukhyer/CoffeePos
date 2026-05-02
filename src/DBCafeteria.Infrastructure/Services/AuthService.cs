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
            throw new InvalidOperationException("Email/telefono y contrasena son obligatorios.");

        var normalized = request.EmailOrPhone.Trim();
        var phone = long.TryParse(new string(normalized.Where(char.IsDigit).ToArray()), out var parsedPhone) ? parsedPhone : (long?)null;

        var cliente = await db.Clientes.FirstOrDefaultAsync(x =>
            x.Activo && !x.EsInvitado && (x.Email == normalized || (phone != null && x.Telefono == phone)), cancellationToken);

        if (cliente?.PasswordHash is null || !passwordHasher.Verify(request.Password, cliente.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales invalidas.");

        return ToAuthResponse(cliente);
    }

    public async Task<AuthResponse> SignupAsync(SignupRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new InvalidOperationException("El nombre es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Email) && string.IsNullOrWhiteSpace(request.Phone))
            throw new InvalidOperationException("El email o telefono es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Password))
            throw new InvalidOperationException("La contrasena es obligatoria.");

        var name = request.Name.Trim();
        var email = string.IsNullOrWhiteSpace(request.Email) ? null : request.Email.Trim().ToLowerInvariant();
        var phone = ParsePhone(request.Phone);

        if (email is not null)
        {
            var emailExists = await db.Clientes.AnyAsync(x =>
                !x.EsInvitado && x.Email != null && x.Email.ToLower() == email, cancellationToken);
            if (emailExists)
                throw new InvalidOperationException("Este email ya esta registrado. Inicia sesion.");
        }

        if (phone is not null)
        {
            var phoneExists = await db.Clientes.AnyAsync(x =>
                !x.EsInvitado && x.Telefono == phone, cancellationToken);
            if (phoneExists)
                throw new InvalidOperationException("Este telefono ya esta registrado. Inicia sesion.");
        }

        var nameExists = await db.Clientes.AnyAsync(x =>
            !x.EsInvitado && x.Nombre != null && x.Nombre.ToLower() == name.ToLowerInvariant(), cancellationToken);
        if (nameExists)
            throw new InvalidOperationException("Este nombre ya esta registrado. Inicia sesion.");

        var cliente = new Cliente
        {
            Nombre = name,
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
            Nombre = string.IsNullOrWhiteSpace(request.Name) ? "Invitado" : request.Name.Trim(),
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
        new(cliente.Id, cliente.Nombre ?? "Invitado", cliente.Email, cliente.Telefono?.ToString(), cliente.EsInvitado, tokenService.CreateAccessToken(cliente), tokenService.CreateRefreshToken());

    private static long? ParsePhone(string? phone)
    {
        var digits = new string((phone ?? string.Empty).Where(char.IsDigit).ToArray());
        return long.TryParse(digits, out var value) ? value : null;
    }
}
