using Obilet.JourneySearch.Web.Models.Api.Journey;

namespace Obilet.JourneySearch.Web.Services.Journey;

public sealed class JourneyService(ObiletApiClient apiClient) : IJourneyService
{
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

        return (response.Data ?? [])
            .OrderBy(journey => journey.Journey?.Departure is null)
            .ThenBy(journey => journey.Journey?.Departure)
            .ToList();
    }
}
