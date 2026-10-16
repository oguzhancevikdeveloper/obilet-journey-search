namespace Obilet.JourneySearch.Tests.Helpers;

/// <summary>
/// Fake HTTP client factory - test için kullanılır
/// </summary>
public sealed class FakeHttpClientFactory : IHttpClientFactory
{
    private readonly HttpClient _httpClient;

    public FakeHttpClientFactory(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public HttpClient CreateClient(string name) => _httpClient;
}
