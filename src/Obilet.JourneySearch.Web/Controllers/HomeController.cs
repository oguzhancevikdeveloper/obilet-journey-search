namespace Obilet.JourneySearch.Web.Controllers;

public class HomeController(IObiletSessionService obiletSessionService,
                            IBusLocationService busLocationService) : Controller
{
    /// <summary>
    /// Kullanılabilir lokasyonlar ve varsayılan arama parametreleriyle ana sayfayı görüntüler.
    /// </summary>
    /// <remarks>
    /// Mevcut HTTP oturumunu kullanarak bir Obilet oturumu oluşturur veya mevcut oturumu korur.
    /// busLocationService aracılığıyla lokasyonları getirir ve HomeViewModel nesnesini
    /// varsayılan kalkış noktası, varış noktası ve yarının tarihiyle başlatır.
    /// </remarks>
    /// <param name="cancellationToken">
    /// Asenkron işlemi iptal etmek için kullanılan iptal belirteci.
    /// </param>
    /// <returns>
    /// Lokasyonları, varsayılan kalkış ve varış noktalarını ve hareket tarihini içeren
    /// HomeViewModel nesnesiyle ana sayfayı görüntüleyen bir IActionResult döndürür.
    /// </returns>
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() == "::1"
            ? "127.0.0.1" : HttpContext.Connection.RemoteIpAddress?.ToString();

        var sessionRequest = new GetSessionRequest(
            Type: 1,
            Connection: new GetSessionConnection(
                IpAddress: ipAddress,
                Port: "5117"),
            Browser: new GetSessionBrowser(
                Name: "Chrome",
                Version: "47.0.0.12"));

        await obiletSessionService.EnsureSessionAsync(
            HttpContext.Session,
            sessionRequest,
            cancellationToken);

        var deviceSession =
            obiletSessionService.GetDeviceSession(HttpContext.Session);

        var locations = await busLocationService.GetAllAsync(
            deviceSession,
            cancellationToken);

        var viewModel = new HomeViewModel
        {
            Locations = locations,
            OriginId = locations.Count > 0 ? locations[0].Id : null,
            OriginName = locations.Count > 0 ? locations[0].Name : null,
            DestinationId = locations.Count > 1 ? locations[1].Id : null,
            DestinationName = locations.Count > 1 ? locations[1].Name : null,
            DepartureDate = DateTime.Today.AddDays(1)
        };


        return View(viewModel);
    }


    /// <summary>
    /// Arama kriterlerini doğrular. Kriterler geçerliyse yolculuklar sayfasına ilgili rota
    /// parametreleriyle yönlendirir; geçersizse doğrulama hatalarıyla birlikte ana sayfayı döndürür.
    /// </summary>
    /// <remarks>
    /// Doğrulama kuralları: Kalkış ve varış noktaları birbirinden farklı olmalıdır; hareket tarihi
    /// bugün veya daha ileri bir tarih olmalıdır. Model geçersiz olduğunda cihaz oturumuna ait
    /// lokasyonlar yeniden alınır ve model doğrulama hatalarıyla birlikte ana sayfa döndürülür.
    /// </remarks>
    /// <param name="model">
    /// Kalkış ve varış noktalarının kimliklerini, görüntülenecek adlarını ve hareket tarihini
    /// içeren arama kriterleri.
    /// </param>
    /// <param name="cancellationToken">
    /// Asenkron işlemi iptal etmek için kullanılan iptal belirteci.
    /// </param>
    /// <returns>
    /// Model geçerliyse rota parametreleriyle Yolculuklar sayfasına yönlendiren; geçersizse
    /// doğrulama hatalarıyla birlikte ana sayfayı döndüren bir IActionResult.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(SearchJourneyViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            if (model.OriginId == model.DestinationId)
            {
                ModelState.AddModelError(
                    nameof(model.DestinationId),
                    "Kalkış ve varış noktaları aynı olamaz.");
            }

            if (model.DepartureDate!.Value.Date < DateTime.Today)
            {
                ModelState.AddModelError(
                    nameof(model.DepartureDate),
                    "Gidiş tarihi bugün veya daha ileri bir tarih olmalıdır.");
            }
        }

        if (!ModelState.IsValid)
        {
            var invalidModelDeviceSession =
                obiletSessionService.GetDeviceSession(HttpContext.Session);

            var invalidModelLocations = await busLocationService.GetAllAsync(
                invalidModelDeviceSession,
                cancellationToken);

            return await ReturnHomeWithErrorsAsync(model, invalidModelLocations);
        }

        return RedirectToAction(
            "Index",
            "Journey",
            new
            {
                originId = model.OriginId,
                originName = model.OriginName,
                destinationId = model.DestinationId,
                destinationName = model.DestinationName,
                departureDate = model.DepartureDate!.Value.ToString("yyyy-MM-dd")
            });
    }


    /// <summary>
    /// Belirtilen anahtar kelimeyle eşleşen otobüs lokasyonlarını arar ve kimlik, ad ve uzun ad bilgilerini içeren bir JSON sonucu döndürür.
    /// </summary>
    /// <remarks>
    /// HttpContext.Session üzerinden cihaz oturumunu alır. busLocationService.SearchAsync
    /// metoduna yapılan çağrıda CancellationToken.None kullanılır.
    /// </remarks>
    /// <param name="keyword">
    /// Lokasyonları filtrelemek için kullanılan arama terimi. Null veya boşluk karakterlerinden
    /// oluşuyorsa boş bir dizi döndürülür.
    /// </param>
    /// <param name="cancellationToken">
    /// İstek için kullanılan iptal belirteci; lokasyon arama çağrısına aktarılmaz.
    /// </param>
    /// <returns>
    /// Her biri id, name ve longName özelliklerini içeren eşleşen lokasyonlardan oluşan
    /// bir diziyi içeren JSON sonucu döndürür.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> SearchLocations(string keyword, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(keyword))
            return Json(Array.Empty<object>());

        var deviceSession =
            obiletSessionService.GetDeviceSession(HttpContext.Session);

        var locations = await busLocationService.SearchAsync(
            deviceSession,
            keyword,
            CancellationToken.None);

        return Json(locations.Select(location => new
        {
            id = location.Id,
            name = location.Name,
            longName = location.LongName
        }));
    }


    /// <summary>
    /// Verilen arama modeli ve lokasyonlar kullanılarak oluşturulan HomeViewModel ile Index görünümünü döndürür.
    /// </summary>
    /// <remarks>
    /// model.DepartureDate değeri null ise DepartureDate özelliği yarının tarihi olarak ayarlanır.
    /// </remarks>
    /// <param name="model">
    /// Seçilen kalkış ve varış noktalarının kimliklerini ve adlarını, ayrıca isteğe bağlı hareket tarihini içeren SearchJourneyViewModel nesnesi.
    /// </param>
    /// <param name="locations">
    /// HomeViewModel nesnesinin Locations özelliğini doldurmak için kullanılan salt okunur BusLocationDto listesi.
    /// </param>
    /// <returns>
    /// Oluşturulan HomeViewModel ile Index görünümünü görüntüleyen bir IActionResult sonucuna dönüşen Task.
    /// </returns>
    private Task<IActionResult> ReturnHomeWithErrorsAsync(SearchJourneyViewModel model, IReadOnlyList<BusLocationDto> locations)
    {
        var viewModel = new HomeViewModel
        {
            Locations = locations,
            OriginId = model.OriginId,
            OriginName = model.OriginName,
            DestinationId = model.DestinationId,
            DestinationName = model.DestinationName,
            DepartureDate = model.DepartureDate ?? DateTime.Today.AddDays(1)
        };

        return Task.FromResult<IActionResult>(View("Index", viewModel));
    }


    /// <summary>
    /// Activity.Current?.Id veya HttpContext.TraceIdentifier üzerinden elde edilen RequestId değerini
    /// içeren bir ErrorViewModel ile hata görünümünü görüntüler.
    /// </summary>
    /// <remarks>
    /// Yanıt önbelleğe alınmaz (ResponseCache: Duration=0, Location=None, NoStore=true).
    /// </remarks>
    /// <returns>
    /// Hata görünümünü görüntüleyen bir IActionResult döndürür.
    /// </returns>
    [ResponseCache(
     Duration = 0,
     Location = ResponseCacheLocation.None,
     NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id
                ?? HttpContext.TraceIdentifier
        });
    }
}
