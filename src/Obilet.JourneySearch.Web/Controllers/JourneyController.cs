using Microsoft.AspNetCore.Mvc;

namespace Obilet.JourneySearch.Web.Controllers;

public class JourneyController : Controller
{
    [HttpGet]
    public IActionResult Index(
        int originId,
        int destinationId,
        DateTime departureDate)
    {
        return Content(
            $"Origin: {originId}, " +
            $"Destination: {destinationId}, " +
            $"Departure: {departureDate:yyyy-MM-dd}");
    }
}