namespace DBCafeteria.Application.DTOs;

public sealed record CategoryDto(int Id, string Name, string Key, double? Price, double? PriceWithTax);
public sealed record ProductDto(int Id, int CategoryId, string Name, bool HotAvailable, bool ColdAvailable, double? Price, double? PriceWithTax);
public sealed record OptionDto(int Id, string Name, double Price, double PriceWithTax);
public sealed record CurrencyDto(string Code, string Symbol);
public sealed record CustomizationDto(
    ProductDto Product,
    CurrencyDto Currency,
    IReadOnlyList<string> Temperatures,
    IReadOnlyList<OptionDto> BeanTypes,
    IReadOnlyList<OptionDto> Sizes,
    IReadOnlyList<OptionDto> Milks,
    IReadOnlyList<OptionDto> Toppings);
