namespace Obilet.JourneySearch.Web.Services.ObiletSession;

/// <summary>
/// Obilet Business API ile oturum yönetimini sağlayan servis.
/// Her kullanıcı için benzersiz SessionId ve DeviceId oluşturur ve session'da saklar.
/// </summary>
public sealed class ObiletSessionService(
    ObiletApiClient apiClient) : IObiletSessionService
{
    /// <summary>
    /// Session tarafında SessionId ve DeviceId için kullanılan key
    /// </summary>
    private const string SessionIdKey = "Obilet.SessionId";

    /// <summary>
    /// Session tarafında SessionId ve DeviceId için kullanılan key
    /// </summary>
    private const string DeviceIdKey = "Obilet.DeviceId";

    /// <summary>
    /// Kullanıcı için Obilet oturumu oluşturur ve session'da saklar.
    /// Eğer oturum zaten varsa yeniden oluşturmaz (idempotent).
    /// </summary>
    /// <param name="session">Geçerli HTTP session nesnesi</param>
    /// <param name="request">GetSession API isteği (IP, browser bilgisi içerir)</param>
    /// <param name="cancellationToken">İptal tokeni</param>
    /// <exception cref="InvalidOperationException">API'den yanıt alınamaz veya geçersizse</exception>
    public async Task EnsureSessionAsync(
        ISession session,
        GetSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var sessionId = session.GetString(SessionIdKey);
        var deviceId = session.GetString(DeviceIdKey);

        // Oturum zaten varsa ve geçerliyse devam etme (idempotent davranış)
        if (!string.IsNullOrWhiteSpace(sessionId) && 
            !string.IsNullOrWhiteSpace(deviceId))
        {
            return;
        }

        // API'ye GetSession isteği gönder
        var response = await apiClient.PostAsync<
            GetSessionRequest,
            GetSessionResponse>(
                "client/getsession",
                request,
                cancellationToken);

        // API yanıtını doğrula
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

        // Session'da sakla - diğer servisler bu bilgiyi kullanabilir
        session.SetString(SessionIdKey, response.Data.SessionId);
        session.SetString(DeviceIdKey, response.Data.DeviceId);
    }

    /// <summary>
    /// Session'dan DeviceSession bilgilerini alır (SessionId + DeviceId).
    /// </summary>
    /// <param name="session">Geçerli HTTP session nesnesi</param>
    /// <returns>SessionId ve DeviceId içeren DeviceSession nesnesi</returns>
    /// <exception cref="InvalidOperationException">Session bilgisi eksikse</exception>
    public DeviceSession GetDeviceSession(ISession session)
    {
        var (sessionId, deviceId) = GetSession(session);

        return new DeviceSession(
            SessionId: sessionId,
            DeviceId: deviceId);

    }

    /// <summary>
    /// Session'dan SessionId ve DeviceId tuple'ı alır.
    /// </summary>
    /// <param name="session">Geçerli HTTP session nesnesi</param>
    /// <returns>(SessionId, DeviceId) tuple'ı</returns>
    /// <exception cref="InvalidOperationException">Session bilgisi eksikse</exception>
    /// <remarks>
    /// Bu metod GetDeviceSession() tarafından içsel olarak kullanılır.
    /// Direkt çağırım gerekirse, dönen tuple'ı DeviceSession'a dönüştürmek için GetDeviceSession() kullanın.
    /// </remarks>
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