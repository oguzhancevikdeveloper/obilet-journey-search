namespace Obilet.JourneySearch.Web.Services;

public sealed class ObiletApiClient(
    IHttpClientFactory httpClientFactory) : IObiletApiClient
{
    private readonly HttpClient _httpClient =
        httpClientFactory.CreateClient("ObiletApi");


    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TResponse : class
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(
                endpoint,
                request,
                cancellationToken);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<TResponse>(
                cancellationToken: cancellationToken);

            return result ?? throw new InvalidOperationException(
                "Obilet API boş veya geçersiz bir yanıt döndürdü.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // İstek çağıran tarafça iptal edildi.
            throw;
        }
        catch (Exception ex)
        {
            // Token veya hassas istek verilerini loglama.
            Console.Error.WriteLine(
                $"Obilet API isteği başarısız. Endpoint: {endpoint}. Hata: {ex.Message}");

            throw;
        }
    }

}
