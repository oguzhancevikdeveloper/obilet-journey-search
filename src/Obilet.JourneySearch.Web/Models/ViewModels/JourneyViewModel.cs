namespace Obilet.JourneySearch.Web.Models.ViewModels;

public sealed record JourneyViewModel(
    int OriginId,
    int DestinationId,
    string OriginName,
    string DestinationName,
    DateTime DepartureDate,
    IReadOnlyList<JourneyDto> Journeys);