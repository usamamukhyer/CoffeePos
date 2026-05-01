using DBCafeteria.Application.DTOs;
using DBCafeteria.Application.Interfaces;
using DBCafeteria.Domain.Entities;
using DBCafeteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DBCafeteria.Infrastructure.Services;

public sealed class GiftService(CafeDbContext db) : IGiftService
{
    public async Task<GiftDto> CreateGiftAsync(int orderId, GiftRequest request, int? senderCustomerId, CancellationToken cancellationToken)
    {
        if (request.RecipientCustomerId <= 0)
            throw new InvalidOperationException("Please select an existing customer.");

        var recipientExists = await db.Clientes
            .AnyAsync(customer => customer.Id == request.RecipientCustomerId && customer.Activo, cancellationToken);
        if (!recipientExists)
            throw new InvalidOperationException("Please select an existing customer.");

        var regalo = new Regalo
        {
            IdPedido = orderId,
            IdClienteRemitente = senderCustomerId,
            IdClienteReceptor = request.RecipientCustomerId,
            NombreReceptor = request.RecipientName,
            TelefonoReceptor = request.RecipientPhone,
            Mensaje = request.Message,
            CodigoRegalo = GenerateGiftCode(),
            Estado = "Pending",
            FechaCreacion = DateTime.Now
        };

        db.Regalos.Add(regalo);
        await db.SaveChangesAsync(cancellationToken);
        return ToDto(regalo);
    }

    public async Task<GiftDto?> GetGiftAsync(int id, CancellationToken cancellationToken)
    {
        var regalo = await db.Regalos.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return regalo is null ? null : ToDto(regalo);
    }

    private static GiftDto ToDto(Regalo regalo) =>
        new(regalo.Id, regalo.IdPedido, regalo.NombreReceptor, regalo.TelefonoReceptor, regalo.Mensaje, regalo.CodigoRegalo, regalo.Estado, regalo.FechaCreacion);

    private static string GenerateGiftCode() => $"GFT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid():N}"[..22].ToUpperInvariant();
}
