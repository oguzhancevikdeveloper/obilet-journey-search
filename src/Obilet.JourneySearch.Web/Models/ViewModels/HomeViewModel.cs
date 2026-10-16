using Obilet.JourneySearch.Web.Models.Api.Location;

namespace Obilet.JourneySearch.Web.Models.ViewModels;

public sealed class HomeViewModel
{
    public IReadOnlyList<BusLocationDto> Locations { get; init; } = [];

    public int? OriginId { get; init; }

    public int? DestinationId { get; init; }

    public DateTime DepartureDate { get; init; } = DateTime.Today.AddDays(1);
}
