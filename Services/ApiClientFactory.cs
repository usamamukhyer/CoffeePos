namespace DBCafeteria.Services;

public static class ApiClientFactory
{
    public static CafeApiClient Create() => new(new HttpClient
    {
        BaseAddress = new Uri(ApiSettings.BaseUrl),
        Timeout = TimeSpan.FromSeconds(20)
    });
}
