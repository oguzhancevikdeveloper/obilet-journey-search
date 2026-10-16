using System.Text.Json.Serialization;

namespace Obilet.JourneySearch.Web.Models.Api.Session;

public sealed record GetSessionResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] GetSessionData? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("user-message")] string? UserMessage
);

public sealed record GetSessionData(
    [property: JsonPropertyName("session-id")] string SessionId,
    [property: JsonPropertyName("device-id")] string DeviceId
);
