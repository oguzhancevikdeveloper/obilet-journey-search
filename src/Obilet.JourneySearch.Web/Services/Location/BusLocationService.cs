using Obilet.JourneySearch.Web.Models.Api.Location;

namespace Obilet.JourneySearch.Web.Services.Location;

/// <summary>
/// Otobus konumları (şehirler) ile ilgili işlemleri yönetir.
/// Obilet API'den şehir listesini alır ve arama işlevselliğini sağlar.
/// </summary>
public sealed class BusLocationService(ObiletApiClient apiClient) : IBusLocationService
{
    /// <summary>
    /// Tüm otobus konumlarını (şehirlerini) alır.
    /// </summary>
    /// <param name="deviceSession">Kullanıcının DeviceSession bilgisi (SessionId + DeviceId)</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Tüm konumların listesi (BusLocationDto)</returns>
    /// <exception cref="InvalidOperationException">API'den geçerli yanıt alınamazsa</exception>
    public Task<IReadOnlyList<BusLocationDto>> GetAllAsync(
        DeviceSession deviceSession,
        CancellationToken cancellationToken = default)
    {
        return GetLocationsAsync(
            deviceSession,
            null,
            cancellationToken);
    }

    /// <summary>
    /// Anahtar kelime ile konumları arar (bazı harfler yazıldıktan sonra filtreleme).
    /// </summary>
    /// <param name="deviceSession">Kullanıcının DeviceSession bilgisi</param>
    /// <param name="keyword">Şehir adının bir parçası (örn: "İs" İstanbul bulur)</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Keyword ile eşleşen konumların listesi</returns>
    /// <remarks>
    /// - Boş veya whitespace keyword verilirse GetAllAsync() çağrılır.
    /// - Arama sunucu tarafında yapılır (filtering frontend'de olabilir).
    /// </remarks>
    public Task<IReadOnlyList<BusLocationDto>> SearchAsync(
        DeviceSession deviceSession,
        string keyword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return GetAllAsync(deviceSession, cancellationToken);
        }

        return GetLocationsAsync(
            deviceSession,
            keyword.Trim(),
            cancellationToken);
    }

    /// <summary>
    /// İç metod: GetBusLocations API uç noktasını çağırarak konumları alır.
    /// </summary>
    /// <param name="deviceSession">Kullanıcının DeviceSession bilgisi</param>
    /// <param name="keyword">Arama terimi (null ise tüm konumlar, string ise filtreleme)</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>Konumların listesi</returns>
    /// <exception cref="InvalidOperationException">API'den boş veya hatalı yanıt alınırsa</exception>
    /// <remarks>
    /// API yanıtı şu format içerir:
    /// - Status: "Success" veya "Error"
    /// - Data: BusLocationDto listesi (Id, Name, LongName içerir)
    /// </remarks>
    private async Task<IReadOnlyList<BusLocationDto>> GetLocationsAsync(
        DeviceSession deviceSession,
        string? keyword,
        CancellationToken cancellationToken)
    {
        var request = new GetBusLocationsRequest(
            Data: keyword,
            DeviceSession: deviceSession,
            Date: DateTime.Now,
            Language: "tr-TR");

        var response = await apiClient.PostAsync<
            GetBusLocationsRequest,
            GetBusLocationsResponse>(
                "location/getbuslocations",
                request,
                cancellationToken);

        if (response is null ||
            !string.Equals(
                response.Status,
                "Success",
                StringComparison.OrdinalIgnoreCase) ||
            response.Data is null)
        {
            throw new InvalidOperationException(
                "Obilet lokasyonları alınamadı.");
        }

        return response.Data;
    }
}
