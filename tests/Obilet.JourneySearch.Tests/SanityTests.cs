using Obilet.JourneySearch.Web.Services;
using Obilet.JourneySearch.Web.Services.Location;
using Obilet.JourneySearch.Web.Services.Journey;
using Obilet.JourneySearch.Web.Services.ObiletSession;
using Microsoft.Extensions.DependencyInjection;

namespace Obilet.JourneySearch.Tests;

/// <summary>
/// Temel sanity testleri - uygulamanın temel özellikleri çalışır durumda mı?
/// </summary>
public class SanityTests
{
    [Fact]
    public void ObiletApiClient_Implements_Interface()
    {
        // Arrange & Act
        var client = typeof(ObiletApiClient);
        var interfaces = client.GetInterfaces();

        // Assert
        Assert.Contains(typeof(IObiletApiClient), interfaces);
    }

    [Fact]
    public void ObiletApiClient_PostAsync_Method_Exists()
    {
        // Arrange & Act
        var method = typeof(IObiletApiClient).GetMethod(
            "PostAsync",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

        // Assert
        Assert.NotNull(method);
        Assert.True(method.IsGenericMethodDefinition);
    }

    [Fact]
    public void IObiletApiClient_Interface_Has_PostAsync()
    {
        // Arrange & Act
        var @interface = typeof(IObiletApiClient);
        var method = @interface.GetMethod("PostAsync");

        // Assert
        Assert.NotNull(method);
    }


}
