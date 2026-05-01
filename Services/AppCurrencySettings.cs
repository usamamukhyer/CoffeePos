namespace DBCafeteria.Services;

public static class AppCurrencySettings
{
    public const string DefaultCurrencyCode = "INR";
    public const string DefaultCurrencySymbol = "₹";

    public static string Format(decimal amount, bool showPlus = false)
    {
        var prefix = showPlus && amount > 0 ? "+" : string.Empty;
        return $"{prefix}{DefaultCurrencySymbol}{amount:0.00}";
    }
}
