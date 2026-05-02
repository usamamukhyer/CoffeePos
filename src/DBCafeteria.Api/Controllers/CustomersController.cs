using DBCafeteria.Application.DTOs;
using DBCafeteria.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBCafeteria.Api.Controllers;

[ApiController]
[Route("api/customers")]
public sealed class CustomersController(CafeDbContext db) : ControllerBase
{
    [HttpGet("sync")]
    public async Task<ActionResult<IReadOnlyList<CustomerSyncItemDto>>> Sync(CancellationToken cancellationToken)
    {
        var customers = await db.Clientes
            .AsNoTracking()
            .OrderBy(customer => customer.Id)
            .Select(customer => new CustomerSyncItemDto(
                customer.Id,
                customer.Nombre ?? string.Empty,
                customer.Telefono.HasValue ? customer.Telefono.Value.ToString() : string.Empty,
                customer.Email,
                customer.EsInvitado,
                customer.Activo,
                customer.Id + "|" + (customer.Nombre ?? string.Empty) + "|" + (customer.Telefono.HasValue ? customer.Telefono.Value.ToString() : string.Empty) + "|" + (customer.Email ?? string.Empty) + "|" + customer.EsInvitado + "|" + customer.Activo + "|" + customer.FechaRegistro))
            .ToListAsync(cancellationToken);

        return Ok(customers);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<CustomerSearchResultDto>>> Search([FromQuery] string? query, CancellationToken cancellationToken)
    {
        var term = query?.Trim();
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
            return Ok(Array.Empty<CustomerSearchResultDto>());

        var customers = await db.Clientes
            .AsNoTracking()
            .Where(customer => customer.Activo)
            .Where(customer =>
                (customer.Nombre != null && customer.Nombre.Contains(term)) ||
                (customer.Email != null && customer.Email.Contains(term)) ||
                (customer.Telefono != null && customer.Telefono.ToString()!.Contains(term)))
            .OrderBy(customer => customer.Nombre)
            .Take(10)
            .Select(customer => new CustomerSearchResultDto(
                customer.Id,
                customer.Nombre ?? string.Empty,
                customer.Telefono.HasValue ? customer.Telefono.Value.ToString() : string.Empty,
                customer.Email))
            .ToListAsync(cancellationToken);

        return Ok(customers);
    }
}
