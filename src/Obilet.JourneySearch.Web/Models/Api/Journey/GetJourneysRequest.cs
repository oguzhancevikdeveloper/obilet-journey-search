using Obilet.JourneySearch.Web.Models.Api.Location;
using System.Text.Json.Serialization;

namespace Obilet.JourneySearch.Web.Models.Api.Journey;

public sealed record GetJourneysRequest(
    [property: JsonPropertyName("device-session")]
    DeviceSession DeviceSession,

    [property: JsonPropertyName("date")]
    DateTime Date,

    [property: JsonPropertyName("language")]
    string Language,

    [property: JsonPropertyName("data")]
    GetJourneysData Data
);

public sealed record GetJourneysData(
    [property: JsonPropertyName("origin-id")]
    int OriginId,

    [property: JsonPropertyName("destination-id")]
    int DestinationId,

    [property: JsonPropertyName("departure-date")]
    string DepartureDate
);
