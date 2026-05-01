using DBCafeteria.Application.DTOs;
using DBCafeteria.Application.Interfaces;
using DBCafeteria.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DBCafeteria.Infrastructure.Services;

public sealed class MenuService(CafeDbContext db) : IMenuService
{
    private const string DefaultCurrencyCode = "INR";
    private const string DefaultCurrencySymbol = "₹";

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken) =>
        await db.Clasificaciones
            .OrderBy(x => x.Descripcion)
            .Select(x => new CategoryDto(x.Id, x.Descripcion, x.Clave, x.Precio, x.PrecioConIVA))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ProductDto>> GetProductsAsync(int? categoryId, CancellationToken cancellationToken)
    {
        var query = db.SubClasificaciones.AsQueryable();
        if (categoryId is not null)
            query = query.Where(x => x.IdClasificacion == categoryId);

        return await query
            .OrderBy(x => x.Descripcion)
            .Select(x => new ProductDto(x.Id, x.IdClasificacion, x.Descripcion, x.BebidasCalientes, x.BebidasFrias, x.Precio, x.PrecioConIVA))
            .ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyList<ProductDto>> GetSubcategoriesAsync(int categoryId, CancellationToken cancellationToken) =>
        GetProductsAsync(categoryId, cancellationToken);

    public async Task<CustomizationDto?> GetCustomizationAsync(int subcategoryId, CancellationToken cancellationToken)
    {
        var product = await db.SubClasificaciones
            .AsNoTracking()
            .Where(x => x.Id == subcategoryId)
            .Select(x => new ProductDto(x.Id, x.IdClasificacion, x.Descripcion, x.BebidasCalientes, x.BebidasFrias, x.Precio, x.PrecioConIVA))
            .FirstOrDefaultAsync(cancellationToken);

        if (product is null)
            return null;

        var temperatures = new List<string>();
        if (product.HotAvailable)
            temperatures.Add("HOT");
        if (product.ColdAvailable)
            temperatures.Add("COLD");

        var milks = await db.SubClasificacionesLeches
            .AsNoTracking()
            .Where(x => x.IdSubClasificacion == subcategoryId)
            .OrderBy(x => x.Leche!.Descripcion)
            .Select(x => new OptionDto(x.IdLeche, x.Leche!.Descripcion ?? x.Leche.Clave ?? $"Milk {x.IdLeche}", x.Leche.Precio, x.Leche.PrecioConIVA))
            .ToListAsync(cancellationToken);

        var beanTypes = await db.SubClasificacionesTiposGranos
            .AsNoTracking()
            .Where(x => x.IdSubClasificacion == subcategoryId)
            .OrderBy(x => x.TipoGrano!.Descripcion)
            .Select(x => new OptionDto(x.IdTipoGrano, x.TipoGrano!.Descripcion ?? x.TipoGrano.Clave ?? $"Bean {x.IdTipoGrano}", x.TipoGrano.Precio, x.TipoGrano.PrecioConIva))
            .ToListAsync(cancellationToken);

        var toppings = await db.SubClasificacionesToppings
            .AsNoTracking()
            .Where(x => x.IdSubClasificacion == subcategoryId)
            .OrderBy(x => x.Topping!.Descripcion)
            .Select(x => new OptionDto(x.IdTopping, x.Topping!.Descripcion, x.Topping.Precio, x.Topping.PrecioConIVA))
            .ToListAsync(cancellationToken);

        var sizes = await db.SubClasificacionesVasos
            .AsNoTracking()
            .Where(x => x.IdSubClasificacion == subcategoryId)
            .OrderBy(x => x.Vaso!.ClaveNumerica)
            .Select(x => new OptionDto(
                x.IdVaso,
                x.Vaso!.Descripcion ?? x.Vaso.Clave ?? $"Cup {x.IdVaso}",
                x.Precio ?? 0,
                x.PrecioConIVA ?? x.Precio ?? 0))
            .ToListAsync(cancellationToken);

        return new CustomizationDto(
            product,
            new CurrencyDto(DefaultCurrencyCode, DefaultCurrencySymbol),
            temperatures,
            beanTypes,
            sizes,
            milks,
            toppings);
    }

    public async Task<IReadOnlyList<OptionDto>> GetMilksAsync(CancellationToken cancellationToken) =>
        await db.Leches.OrderBy(x => x.Descripcion).Select(x => new OptionDto(x.Id, x.Descripcion ?? x.Clave ?? $"Milk {x.Id}", x.Precio, x.PrecioConIVA)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OptionDto>> GetBeanTypesAsync(CancellationToken cancellationToken) =>
        await db.TiposGranos.OrderBy(x => x.Descripcion).Select(x => new OptionDto(x.Id, x.Descripcion ?? x.Clave ?? $"Bean {x.Id}", x.Precio, x.PrecioConIva)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OptionDto>> GetCupsAsync(CancellationToken cancellationToken) =>
        await db.Vasos.OrderBy(x => x.ClaveNumerica).Select(x => new OptionDto(x.Id, x.Descripcion ?? x.Clave ?? $"Cup {x.Id}", 0, 0)).ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<OptionDto>> GetToppingsAsync(CancellationToken cancellationToken) =>
        await db.Toppings.OrderBy(x => x.Descripcion).Select(x => new OptionDto(x.Id, x.Descripcion, x.Precio, x.PrecioConIVA)).ToListAsync(cancellationToken);
}
