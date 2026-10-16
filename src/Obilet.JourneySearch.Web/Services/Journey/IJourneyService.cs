namespace Obilet.JourneySearch.Web.Services.Journey;

public interface IJourneyService
{
    Task<IReadOnlyList<JourneyDto>> GetJourneysAsync(
        string sessionId,
        string deviceId,
        int originId,
        int destinationId,
        DateTime departureDate,
        CancellationToken cancellationToken = default);
}