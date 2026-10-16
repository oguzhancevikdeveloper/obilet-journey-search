namespace Obilet.JourneySearch.Web.Services.ObiletSession;

public interface IObiletSessionService
{

    Task EnsureSessionAsync(ISession session,GetSessionRequest request,
        CancellationToken cancellationToken = default);

    (string SessionId, string DeviceId) GetSession(ISession session);

    DeviceSession GetDeviceSession(ISession session);
}