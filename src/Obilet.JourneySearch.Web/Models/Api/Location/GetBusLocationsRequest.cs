using System.Text.Json.Serialization;

namespace Obilet.JourneySearch.Web.Models.Api.Location;

public sealed record GetBusLocationsRequest(
    [property: JsonPropertyName("data")] string? Data,
    [property: JsonPropertyName("device-session")] DeviceSession DeviceSession,
    [property: JsonPropertyName("date")] DateTime Date,
    [property: JsonPropertyName("language")] string Language
);

public sealed record DeviceSession(
    [property: JsonPropertyName("session-id")] string SessionId,
    [property: JsonPropertyName("device-id")] string DeviceId
);
