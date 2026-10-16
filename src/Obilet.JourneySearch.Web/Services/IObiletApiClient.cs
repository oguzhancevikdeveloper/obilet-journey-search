namespace Obilet.JourneySearch.Web.Services;

/// <summary>
/// Obilet API ile iletişim kurmak için interface
/// </summary>
public interface IObiletApiClient
{
    /// <summary>
    /// API'ye generic POST isteği gönder
    /// </summary>
    Task<TResponse> PostAsync<TRequest, TResponse>(
        string endpoint,
        TRequest request,
        CancellationToken cancellationToken = default)
        where TResponse : class;
}
