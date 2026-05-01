using DBCafeteria.Application.DTOs;
using DBCafeteria.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DBCafeteria.Api.Controllers;

[ApiController]
[Route("api/branches")]
public sealed class BranchesController(CafeDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<BranchDto>>> GetActiveBranches(CancellationToken cancellationToken)
    {
        var branches = await db.Sucursales
            .AsNoTracking()
            .Where(branch => branch.Activo)
            .OrderBy(branch => branch.Nombre)
            .Select(branch => new BranchDto(branch.Id, branch.Nombre, branch.Direccion))
            .ToListAsync(cancellationToken);

        return Ok(branches);
    }
}
