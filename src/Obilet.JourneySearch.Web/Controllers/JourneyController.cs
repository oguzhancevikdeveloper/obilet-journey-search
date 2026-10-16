namespace Obilet.JourneySearch.Web.Controllers;

public class JourneyController(IJourneyService journeyService,
                               IBusLocationService busLocationService,
                               IObiletSessionService obiletSessionService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> Index(
        int originId,
        int destinationId,
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

            var locations = await busLocationService.GetAllAsync(
                deviceSession,
                cancellationToken);

            var originName = locations
                .FirstOrDefault(location => location.Id == originId)?.Name;

            var destinationName = locations
                .FirstOrDefault(location => location.Id == destinationId)?.Name;

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
                originName ?? originId.ToString(),
                destinationName ?? destinationId.ToString(),
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