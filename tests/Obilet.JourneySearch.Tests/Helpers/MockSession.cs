using Microsoft.AspNetCore.Http;

namespace Obilet.JourneySearch.Tests.Helpers;

/// <summary>
/// Birim testlerde HttpContext.Session mock'lamak için kullanılan yardımcı sınıf
/// </summary>
public class MockSession : ISession
{
    private readonly Dictionary<string, byte[]> _sessionData = new();

    public string Id => "mock-session-id";

    public IEnumerable<string> Keys => _sessionData.Keys;

    public bool IsAvailable => true;

    public void Set(string key, byte[] value)
    {
        _sessionData[key] = value;
    }

    public bool TryGetValue(string key, out byte[] value)
    {
        return _sessionData.TryGetValue(key, out value);
    }

    public void Remove(string key)
    {
        _sessionData.Remove(key);
    }

    public void Clear()
    {
        _sessionData.Clear();
    }

    public Task LoadAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task CommitAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
