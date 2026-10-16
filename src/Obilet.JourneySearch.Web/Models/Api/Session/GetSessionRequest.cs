using System.Text.Json.Serialization;

namespace Obilet.JourneySearch.Web.Models.Api.Session;

public sealed record GetSessionRequest(
    [property: JsonPropertyName("type")] int Type,
    [property: JsonPropertyName("connection")] GetSessionConnection Connection,
    [property: JsonPropertyName("browser")] GetSessionBrowser Browser
);

public sealed record GetSessionConnection(
    [property: JsonPropertyName("ip-address")] string IpAddress,
    [property: JsonPropertyName("port")] string Port
);

public sealed record GetSessionBrowser(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("version")] string Version
);