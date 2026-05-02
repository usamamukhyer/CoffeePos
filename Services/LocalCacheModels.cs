namespace DBCafeteria.Services;

public sealed record LocalCategoryRow(int Id, string Name, string Key, double? Price, double? PriceWithTax, string Version);
public sealed record LocalProductRow(int Id, int CategoryId, string Name, bool HotAvailable, bool ColdAvailable, double? Price, double? PriceWithTax, string Version);
public sealed record LocalOptionRow(string OptionType, int Id, string Name, double Price, double PriceWithTax, string Version);
public sealed record LocalBranchRow(int Id, string Name, string? Address, string Version);
public sealed record LocalCustomerRow(int Id, string Name, string Phone, string? Email, bool IsGuest, bool Active, string Version);
public sealed record LocalSyncQueueRow(string ClientOrderId, string PayloadJson, string Status, int AttemptCount, DateTime CreatedAt, DateTime UpdatedAt, string? LastError);
