using Obilet.JourneySearch.Web.Models.Api.Location;

namespace Obilet.JourneySearch.Web.Services.Location;

public interface IBusLocationService
{
    Task<IReadOnlyList<BusLocationDto>> GetAllAsync(
        DeviceSession deviceSession,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<BusLocationDto>> SearchAsync(
        DeviceSession deviceSession,
        string keyword,
        CancellationToken cancellationToken = default);
}