using System.Net;
using System.Text;

namespace Obilet.JourneySearch.Tests.Helpers;

/// <summary>
/// Gerçek HTTP isteklerini fake JSON yanıtı ile replace eden handler
/// </summary>
public sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly string _jsonResponse;
    private readonly Func<HttpRequestMessage, bool>? _requestValidator;

    public FakeHttpMessageHandler(
        string jsonResponse,
        Func<HttpRequestMessage, bool>? requestValidator = null)
    {
        _jsonResponse = jsonResponse;
        _requestValidator = requestValidator;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        _requestValidator?.Invoke(request);

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(
                _jsonResponse,
                Encoding.UTF8,
                "application/json")
        };

        return Task.FromResult(response);
    }
}
