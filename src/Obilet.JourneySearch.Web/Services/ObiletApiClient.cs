namespace Obilet.JourneySearch.Web.Services;

public sealed class ObiletApiClient(
    IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient =
        httpClientFactory.CreateClient("ObiletApi");

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            endpoint,
            request,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TResponse>(
            cancellationToken: cancellationToken);
    }
}
