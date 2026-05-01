using DBCafeteria.Models;

namespace DBCafeteria.Services;

public sealed class BranchApiService
{
    private readonly CafeApiClient _apiClient;

    public BranchApiService()
        : this(ApiClientFactory.Create())
    {
    }

    public BranchApiService(CafeApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<IReadOnlyList<BranchModel>> GetBranchesAsync(CancellationToken cancellationToken = default)
    {
        var branches = await _apiClient.GetAsync<List<ApiBranch>>("api/branches", cancellationToken);
        return branches?
            .Select(branch => new BranchModel
            {
                Id = branch.Id,
                Name = branch.Name,
                Address = branch.Address
            })
            .ToList() ?? [];
    }
}
