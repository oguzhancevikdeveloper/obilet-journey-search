using Obilet.JourneySearch.Web.Models.Api.Location;
using Obilet.JourneySearch.Web.Models.Api.Session;

namespace Obilet.JourneySearch.Web.Services.ObiletSession;

public sealed class ObiletSessionService(
    ObiletApiClient apiClient) : IObiletSessionService
{
    private const string SessionIdKey = "Obilet.SessionId";
    private const string DeviceIdKey = "Obilet.DeviceId";

    public async Task EnsureSessionAsync(
        ISession session,
        GetSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var sessionId = session.GetString(SessionIdKey);
        var deviceId = session.GetString(DeviceIdKey);

        if (!string.IsNullOrWhiteSpace(sessionId) &&
            !string.IsNullOrWhiteSpace(deviceId))
        {
            return;
        }

        var response = await apiClient.PostAsync<
            GetSessionRequest,
            GetSessionResponse>(
                "client/getsession",
                request,
                cancellationToken);

        if (response is null ||
            !string.Equals(
                response.Status,
                "Success",
                StringComparison.OrdinalIgnoreCase) ||
            response.Data is null ||
            string.IsNullOrWhiteSpace(response.Data.SessionId) ||
            string.IsNullOrWhiteSpace(response.Data.DeviceId))
        {
            throw new InvalidOperationException(
                "Obilet oturumu oluşturulamadı.");
        }

        session.SetString(SessionIdKey, response.Data.SessionId);
        session.SetString(DeviceIdKey, response.Data.DeviceId);
    }

    public DeviceSession GetDeviceSession(ISession session)
    {
        var (sessionId, deviceId) = GetSession(session);

        return new DeviceSession(
            SessionId: sessionId,
            DeviceId: deviceId);

    }

    public (string SessionId, string DeviceId) GetSession(
        ISession session)
    {
        var sessionId = session.GetString(SessionIdKey);
        var deviceId = session.GetString(DeviceIdKey);

        if (string.IsNullOrWhiteSpace(sessionId) ||
            string.IsNullOrWhiteSpace(deviceId))
        {
            throw new InvalidOperationException(
                "Obilet oturumu bulunamadı.");
        }

        return (sessionId, deviceId);
    }

}