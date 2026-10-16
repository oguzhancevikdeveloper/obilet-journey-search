using Obilet.JourneySearch.Web.Models.Api.Location;

namespace Obilet.JourneySearch.Web.Services.Location;

public sealed class BusLocationService(ObiletApiClient apiClient) : IBusLocationService
{
    public Task<IReadOnlyList<BusLocationDto>> GetAllAsync(
        DeviceSession deviceSession,
        CancellationToken cancellationToken = default)
    {
        return GetLocationsAsync(
            deviceSession,
            null,
            cancellationToken);
    }

    public Task<IReadOnlyList<BusLocationDto>> SearchAsync(
        DeviceSession deviceSession,
        string keyword,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return GetAllAsync(deviceSession, cancellationToken);
        }

        return GetLocationsAsync(
            deviceSession,
            keyword.Trim(),
            cancellationToken);
    }

    private async Task<IReadOnlyList<BusLocationDto>> GetLocationsAsync(
        DeviceSession deviceSession,
        string? keyword,
        CancellationToken cancellationToken)
    {
        var request = new GetBusLocationsRequest(
            Data: keyword,
            DeviceSession: deviceSession,
            Date: DateTime.Now,
            Language: "tr-TR");

        var response = await apiClient.PostAsync<
            GetBusLocationsRequest,
            GetBusLocationsResponse>(
                "location/getbuslocations",
                request,
                cancellationToken);

        if (response is null ||
            !string.Equals(
                response.Status,
                "Success",
                StringComparison.OrdinalIgnoreCase) ||
            response.Data is null)
        {
            throw new InvalidOperationException(
                "Obilet lokasyonları alınamadı.");
        }

        return response.Data;
    }
}
