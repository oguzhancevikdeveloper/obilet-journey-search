using Obilet.JourneySearch.Web.Models.ViewModels;

namespace Obilet.JourneySearch.Web.Controllers;

public class HomeController(IObiletSessionService obiletSessionService,
                            IBusLocationService busLocationService) : Controller
{
    public async Task<IActionResult> Index(
        CancellationToken cancellationToken)
    {
        var ipAddress = "165.114.41.21";
        //HttpContext.Connection.RemoteIpAddress?.ToString()
        //?? "127.0.0.1";

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

    public IActionResult Privacy()
    {
        return View();
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
