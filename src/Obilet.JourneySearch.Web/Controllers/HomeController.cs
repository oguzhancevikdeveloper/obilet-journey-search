namespace Obilet.JourneySearch.Web.Controllers;

public class HomeController(IObiletSessionService obiletSessionService,
                            IBusLocationService busLocationService) : Controller
{
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {

        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() == "::1"
            ? "127.0.0.1"
            : HttpContext.Connection.RemoteIpAddress?.ToString();

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
            DestinationId = locations.Count > 1 ? locations[1].Id : null,
            DepartureDate = DateTime.Today.AddDays(1)
        };


        return View(viewModel);
    }





    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(
        SearchJourneyViewModel model,
        CancellationToken cancellationToken)
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
                destinationId = model.DestinationId,
                departureDate = model.DepartureDate!.Value.ToString("yyyy-MM-dd")
            });
    }



    [HttpGet]
    public async Task<IActionResult> SearchLocations(
    string keyword,
    CancellationToken cancellationToken)
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

    private Task<IActionResult> ReturnHomeWithErrorsAsync(
        SearchJourneyViewModel model,
        IReadOnlyList<BusLocationDto> locations)
    {
        var viewModel = new HomeViewModel
        {
            Locations = locations,
            OriginId = model.OriginId,
            DestinationId = model.DestinationId,
            DepartureDate = model.DepartureDate ?? DateTime.Today.AddDays(1)
        };

        return Task.FromResult<IActionResult>(View("Index", viewModel));
    }


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
