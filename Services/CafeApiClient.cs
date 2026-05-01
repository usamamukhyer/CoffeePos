using System.Net.Http.Json;
using System.Text.Json;

namespace DBCafeteria.Services;

public sealed class CafeApiClient(HttpClient httpClient)
{
    public Task<TResponse?> GetAsync<TResponse>(string route, CancellationToken cancellationToken = default) =>
        httpClient.GetFromJsonAsync<TResponse>(route, cancellationToken);

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string route, TRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PostAsJsonAsync(route, request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var message = await ReadErrorMessageAsync(response, cancellationToken);
            throw new ApiException(message, (int)response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(body))
            return $"Request failed with status {(int)response.StatusCode}.";

        try
        {
            using var json = JsonDocument.Parse(body);
            if (json.RootElement.TryGetProperty("message", out var message))
                return message.GetString() ?? body;
        }
        catch (JsonException)
        {
            // Plain-text or unexpected error shape.
        }

        return body;
    }
}

public sealed class ApiException(string message, int statusCode) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
}
