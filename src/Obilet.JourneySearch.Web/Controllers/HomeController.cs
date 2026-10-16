namespace Obilet.JourneySearch.Web.Controllers;

public class HomeController(IObiletSessionService obiletSessionService,
                            IBusLocationService busLocationService) : Controller
{
    /// <summary>
    /// Asynchronously prepares and returns the home view populated with available bus locations and default origin,
    /// destination, and a departure date set to tomorrow.
    /// </summary>
    /// <remarks>Normalizes the client IP address, ensures an external session via obiletSessionService,
    /// retrieves the device session and locations from busLocationService, and constructs the HomeViewModel used by the
    /// view.</remarks>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An IActionResult that renders the home view with a HomeViewModel containing retrieved locations, default
    /// origin/destination selections, and the departure date.</returns>
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
            OriginName = locations.Count > 0 ? locations[0].Name : null,
            DestinationId = locations.Count > 1 ? locations[1].Id : null,
            DestinationName = locations.Count > 1 ? locations[1].Name : null,
            DepartureDate = DateTime.Today.AddDays(1)
        };


        return View(viewModel);
    }




    /// <summary>
    /// Validates search input, enforces business rules, and navigates to the Journey index when valid; otherwise
    /// returns the search form with validation errors.
    /// </summary>
    /// <remarks>HTTP POST action protected by anti-forgery validation. Performs ModelState validation,
    /// ensures OriginId and DestinationId differ, and enforces DepartureDate not earlier than today. On invalid model,
    /// retrieves locations and returns the home view with errors.</remarks>
    /// <param name="model">SearchJourneyViewModel containing search criteria and display values (OriginId, DestinationId, OriginName,
    /// DestinationName, DepartureDate).</param>
    /// <param name="cancellationToken">Token used to cancel the asynchronous operation.</param>
    /// <returns>An IActionResult that redirects to the Journey index with route values when the model is valid, or returns the
    /// search form populated with validation errors when invalid.</returns>
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
                originName = model.OriginName,
                destinationId = model.DestinationId,
                destinationName = model.DestinationName,
                departureDate = model.DepartureDate!.Value.ToString("yyyy-MM-dd")
            });
    }


    /// <summary>
    /// Searches for locations that match the specified keyword and returns a JSON array of objects with id, name, and
    /// longName.
    /// </summary>
    /// <remarks>If keyword is null or whitespace an empty array is returned. The device session is obtained
    /// from HttpContext.Session. The underlying search call is invoked with CancellationToken.None.</remarks>
    /// <param name="keyword">Search term used to match locations.</param>
    /// <param name="cancellationToken">Cancellation token for the request; not forwarded to the underlying location search call.</param>
    /// <returns>A JSON result containing an array of matching locations, each object with properties id, name, and longName.</returns>
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
            OriginName = model.OriginName,
            DestinationId = model.DestinationId,
            DestinationName = model.DestinationName,
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
