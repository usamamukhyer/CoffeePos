using DBCafeteria.Application.DTOs;

namespace DBCafeteria.Application.Interfaces;

public interface IMenuService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductDto>> GetProductsAsync(int? categoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<ProductDto>> GetSubcategoriesAsync(int categoryId, CancellationToken cancellationToken);
    Task<CustomizationDto?> GetCustomizationAsync(int subcategoryId, CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> GetMilksAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> GetBeanTypesAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> GetCupsAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<OptionDto>> GetToppingsAsync(CancellationToken cancellationToken);
}
