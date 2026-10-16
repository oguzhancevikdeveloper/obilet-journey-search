using Obilet.JourneySearch.Web.Models.Api.Journey;

namespace Obilet.JourneySearch.Web.Services.Journey;

/// <summary>
/// Otobus seferlerini (yazılı rota ve zaman tablosed) yönetir.
/// Belirtilen kalkış, varış ve tarih için mevcut seferleri alır.
/// </summary>
public sealed class JourneyService(ObiletApiClient apiClient) : IJourneyService
{
    /// <summary>
    /// Belirtilen parametreler için mevcut otobus seferlerini alır.
    /// </summary>
    /// <param name="sessionId">Kullanıcının Obilet SessionId (API'nin session tracking'ı için)</param>
    /// <param name="deviceId">Kullanıcının Obilet DeviceId (API'nin device tracking'ı için)</param>
    /// <param name="originId">Kalkış noktasının ID'si (şehir ID'si)</param>
    /// <param name="destinationId">Varış noktasının ID'si (şehir ID'si)</param>
    /// <param name="departureDate">Seyahat tarihi (DateTime.Today veya sonrası)</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <returns>
    /// Seferlerin listesi (hareket saatine göre sıralanmış):
    /// - Geçerli departure time'ı olanlar önce (08:00, 14:00, 20:00 sırasında)
    /// - Departure time'ı null olanlar sonunda
    /// </returns>
    /// <exception cref="InvalidOperationException">API'den hatalı yanıt alınırsa veya sefer bulunamazsa</exception>
    /// <remarks>
    /// Sıralama mantığı:
    /// .OrderBy(j => j.Journey?.Departure is null)  // N'ller sonunda (false=0 true=1)
    /// .ThenBy(j => j.Journey?.Departure)            // Geçerli saatler artan sırada
    /// </remarks>
    public async Task<IReadOnlyList<JourneyDto>> GetJourneysAsync(
        string sessionId,
        string deviceId,
        int originId,
        int destinationId,
        DateTime departureDate,
        CancellationToken cancellationToken = default)
    {
        var date = departureDate.ToString("yyyy-MM-dd");

        var request = new GetJourneysRequest(
            DeviceSession: new Models.Api.Location.DeviceSession(
                SessionId: sessionId,
                DeviceId: deviceId),
            Date: departureDate.Date,
            Language: "tr-TR",
            Data: new GetJourneysData(
                OriginId: originId,
                DestinationId: destinationId,
                DepartureDate: date));

        var response = await apiClient.PostAsync<
            GetJourneysRequest,
            GetJourneysResponse>(
                "journey/getbusjourneys",
                request,
                cancellationToken);

        if (!string.Equals(response.Status, "Success", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                response.UserMessage ?? response.Message ?? "Seferler alınamadı.");
        }

        // Seferler hareket saatine göre sıralanır:
        // 1. Null departure'ı olanlar sonunda (is null = true=1, false=0)
        // 2. Geçerli departure'ı olanlar artan sırada (08:00 < 14:00 < 20:00)
        return (response.Data ?? [])
            .OrderBy(journey => journey.Journey?.Departure is null)
            .ThenBy(journey => journey.Journey?.Departure)
            .ToList();
    }
}
