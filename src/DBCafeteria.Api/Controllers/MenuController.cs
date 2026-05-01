using DBCafeteria.Application.DTOs;
using DBCafeteria.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DBCafeteria.Api.Controllers;

[ApiController]
public sealed class MenuController(IMenuService menuService) : ControllerBase
{
    [HttpGet("categories")]
    [HttpGet("api/categories")]
    public async Task<IReadOnlyList<CategoryDto>> Categories(CancellationToken cancellationToken) =>
        await menuService.GetCategoriesAsync(cancellationToken);

    [HttpGet("products")]
    public async Task<IReadOnlyList<ProductDto>> Products([FromQuery] int? categoryId, CancellationToken cancellationToken) =>
        await menuService.GetProductsAsync(categoryId, cancellationToken);

    [HttpGet("api/categories/{idClasificacion:int}/subcategories")]
    public async Task<IReadOnlyList<ProductDto>> Subcategories(int idClasificacion, CancellationToken cancellationToken) =>
        await menuService.GetSubcategoriesAsync(idClasificacion, cancellationToken);

    [HttpGet("api/menu/customization/{idSubClasificacion:int}")]
    public async Task<ActionResult<CustomizationDto>> Customization(int idSubClasificacion, CancellationToken cancellationToken)
    {
        var customization = await menuService.GetCustomizationAsync(idSubClasificacion, cancellationToken);
        return customization is null ? NotFound() : Ok(customization);
    }

    [HttpGet("menu/milks")]
    public async Task<IReadOnlyList<OptionDto>> Milks(CancellationToken cancellationToken) =>
        await menuService.GetMilksAsync(cancellationToken);

    [HttpGet("menu/beans")]
    public async Task<IReadOnlyList<OptionDto>> Beans(CancellationToken cancellationToken) =>
        await menuService.GetBeanTypesAsync(cancellationToken);

    [HttpGet("menu/cups")]
    public async Task<IReadOnlyList<OptionDto>> Cups(CancellationToken cancellationToken) =>
        await menuService.GetCupsAsync(cancellationToken);

    [HttpGet("menu/toppings")]
    public async Task<IReadOnlyList<OptionDto>> Toppings(CancellationToken cancellationToken) =>
        await menuService.GetToppingsAsync(cancellationToken);
}
