namespace Obilet.JourneySearch.Web.Services;

/// <summary>
/// Obilet Business API ile iletişim kuran HTTP istemci sınıfı.
/// Tüm API çağrılarını merkezi olarak yönetir ve hata işlemesini sağlar.
/// </summary>
public sealed class ObiletApiClient(
    IHttpClientFactory httpClientFactory) : IObiletApiClient
{
    private readonly HttpClient _httpClient =
        httpClientFactory.CreateClient("ObiletApi");

    /// <summary>
    /// Obilet API'sine asenkron POST isteği gönderir ve yanıtı döner.
    /// </summary>
    /// <typeparam name="TRequest">İstek JSON modeli tipi</typeparam>
    /// <typeparam name="TResponse">Yanıt JSON modeli tipi (class olmalı)</typeparam>
    /// <param name="endpoint">Hedef API endpoint yolu (örn: "client/getsession", "journey/getbusjourneys")</param>
    /// <param name="request">API'ye gönderilecek istek nesnesi (JSON'a serialized edilecek)</param>
    /// <param name="cancellationToken">İşlemi iptal etmek için kullanılacak CancellationToken</param>
    /// <returns>API'den dönen TResponse tipinde nesne</returns>
    /// <exception cref="HttpRequestException">HTTP isteği başarısız olursa (network hatası, timeout, vb.)</exception>
    /// <exception cref="InvalidOperationException">API yanıtı boş veya geçersizse</exception>
    /// <remarks>
    /// - Sensitive veriler (token, session ID) console'a loglanmaz.
    /// - Cancellation token'ı iptal edilmişse OperationCanceledException fırlatılır.
    /// - Tüm HTTP hataları wrap'lenip rethrow edilir.
    /// </remarks>
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
            // İstek çağıran tarafça iptal edildi - exception'ı propagate et.
            throw;
        }
        catch (Exception ex)
        {
            // Token veya hassas istek verilerini loglama - yalnızca endpoint ve genel hata mesajı.
            // Sensitive info (SessionId, DeviceId, vb.) log'lanmaz.
            Console.Error.WriteLine(
                $"Obilet API isteği başarısız. Endpoint: {endpoint}. Hata: {ex.Message}");

            throw;
        }
    }

}
