namespace Obilet.JourneySearch.Web.Controllers;

public class JourneyController(IJourneyService journeyService,
                               IBusLocationService busLocationService,
                               IObiletSessionService obiletSessionService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Index(
        int originId,
        string originName,
        int destinationId,
        string destinationName,
        DateTime departureDate,
        CancellationToken cancellationToken)
    {
        if (originId == destinationId)
        {
            return BadRequest("Kalkış ve varış noktaları aynı olamaz.");
        }

        if (departureDate.Date < DateTime.Today)
        {
            return BadRequest("Gidiş tarihi bugün veya ileri bir tarih olmalıdır.");
        }

        try
        {
            var deviceSession = obiletSessionService.GetDeviceSession(
                HttpContext.Session);

            var resolvedOriginName = originName ?? originId.ToString();
            var resolvedDestinationName = destinationName ?? destinationId.ToString();

            var journeys = await journeyService.GetJourneysAsync(
                deviceSession.SessionId,
                deviceSession.DeviceId,
                originId,
                destinationId,
                departureDate,
                cancellationToken);

            var viewModel = new JourneyViewModel(
                originId,
                destinationId,
                resolvedOriginName,
                resolvedDestinationName,
                departureDate,
                journeys);

            return View(viewModel);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Kullanıcının isteği iptal edildi; yeni bir hata sayfası üretme.
            throw;
        }
        catch (HttpRequestException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Sefer bilgilerine şu anda ulaşılamıyor. Lütfen daha sonra tekrar deneyin.");
        }
        catch (InvalidOperationException)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Sefer bilgileri şu anda alınamıyor. Lütfen tekrar deneyin.");
        }
        catch (System.Text.Json.JsonException)
        {
            return StatusCode(
                StatusCodes.Status502BadGateway,
                "Sefer bilgileri işlenirken bir sorun oluştu.");
        }
    }

}