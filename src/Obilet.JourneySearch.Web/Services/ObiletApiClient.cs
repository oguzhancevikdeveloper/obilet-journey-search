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
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                endpoint,
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<TResponse>(
                cancellationToken: cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine(
                $"Obilet isteği çağıran tarafça iptal edildi: {endpoint}");

            throw;
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine(
                $"Obilet isteği timeout veya başka bir iptal nedeniyle sonlandı: {endpoint}. {ex.Message}");

            throw;
        }
    }

}
