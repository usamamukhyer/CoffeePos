using DBCafeteria.Models;

namespace DBCafeteria.Services;

public sealed class CustomerApiService
{
    private readonly CafeApiClient _apiClient;

    public CustomerApiService()
        : this(ApiClientFactory.Create())
    {
    }

    public CustomerApiService(CafeApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IReadOnlyList<CustomerSearchResultModel>> SearchCustomersAsync(string query, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 2)
            return [];

        var encodedQuery = Uri.EscapeDataString(query.Trim());
        var results = await _apiClient.GetAsync<List<ApiCustomerSearchResult>>($"api/customers/search?query={encodedQuery}", cancellationToken);

        return results?
            .Select(customer => new CustomerSearchResultModel
            {
                Id = customer.Id,
                Name = customer.Name,
                Phone = customer.Phone,
                Email = customer.Email
            })
            .ToList() ?? [];
    }
}
