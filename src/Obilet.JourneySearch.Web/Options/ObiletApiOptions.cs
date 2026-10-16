namespace Obilet.JourneySearch.Web.Options;

public sealed class ObiletApiOptions
{
    public const string SectionName = "ObiletApi";

    public string BaseUrl { get; init; } = string.Empty;

    public string ApiClientToken { get; init; } = string.Empty;

    public string ApplicationVersion { get; init; } = "1.0.0.0";

    public string EquipmentId { get; init; } = "distribusion";

    public string Language { get; init; } = "en-EN";
}