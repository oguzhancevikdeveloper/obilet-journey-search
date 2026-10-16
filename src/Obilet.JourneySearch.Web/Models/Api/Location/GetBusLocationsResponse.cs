using System.Text.Json.Serialization;

namespace Obilet.JourneySearch.Web.Models.Api.Location;

public sealed record GetBusLocationsResponse(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("data")] List<BusLocationDto>? Data,
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("user-message")] string? UserMessage
);

public sealed record BusLocationDto(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("long-name")] string? LongName,
    [property: JsonPropertyName("rank")] int Rank
);
