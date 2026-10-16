namespace Obilet.JourneySearch.Web.Options;

/// <summary>
/// Obilet API istemcisi için yapılandırma seçeneklerini temsil eder.
/// </summary>
/// <remarks>
/// ObiletApi yapılandırma bölümü kullanılarak ayarlanır. Özellikler yalnızca başlatma
/// aşamasında değer atanabilecek şekilde tanımlanmıştır ve yapılandırma sağlayıcıları
/// tarafından doldurulmak üzere tasarlanmıştır. Varsayılan değerler: BaseUrl ve
/// ApiClientToken boş; ApplicationVersion = 1.0.0.0; EquipmentId = distribusion;
/// Language = en-EN şeklindedir. Obilet API ile iletişim kuran HTTP istemcilerini
/// veya servisleri yapılandırmak için kullanılır.
/// </remarks>
public sealed class ObiletApiOptions
{
    public const string SectionName = "ObiletApi";

    public string BaseUrl { get; init; } = string.Empty;

    public string ApiClientToken { get; init; } = string.Empty;

    public string ApplicationVersion { get; init; } = "1.0.0.0";

    public string EquipmentId { get; init; } = "distribusion";

    public string Language { get; init; } = "en-EN";
}