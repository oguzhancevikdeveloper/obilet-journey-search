using System.Text.Json.Serialization;

namespace Obilet.JourneySearch.Web.Models.Api.Journey;

public sealed record GetJourneysResponse(
    [property: JsonPropertyName("status")]
    string Status,

    [property: JsonPropertyName("data")]
    List<JourneyDto>? Data,

    [property: JsonPropertyName("message")]
    string? Message,

    [property: JsonPropertyName("user-message")]
    string? UserMessage
);

public sealed record JourneyDto(
    [property: JsonPropertyName("id")]
    long Id,

    [property: JsonPropertyName("partner-name")]
    string? PartnerName,

    [property: JsonPropertyName("bus-type")]
    string? BusType,

    [property: JsonPropertyName("available-seats")]
    int? AvailableSeats,

    [property: JsonPropertyName("journey")]
    JourneyDetailDto? Journey,

    [property: JsonPropertyName("origin-location")]
    string? OriginLocation,

    [property: JsonPropertyName("destination-location")]
    string? DestinationLocation
);

public sealed record JourneyDetailDto(
    [property: JsonPropertyName("origin")]
    string? Origin,

    [property: JsonPropertyName("destination")]
    string? Destination,

    [property: JsonPropertyName("departure")]
    DateTime? Departure,

    [property: JsonPropertyName("arrival")]
    DateTime? Arrival,

    [property: JsonPropertyName("duration")]
    string? Duration,

    [property: JsonPropertyName("currency")]
    string? Currency,

    [property: JsonPropertyName("internet-price")]
    decimal? InternetPrice
);