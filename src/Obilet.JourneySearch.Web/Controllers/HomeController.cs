using Microsoft.AspNetCore.Mvc;
using Obilet.JourneySearch.Web.Models;
using Obilet.JourneySearch.Web.Models.Api.Session;
using Obilet.JourneySearch.Web.Services.ObiletSession;
using System.Diagnostics;

namespace Obilet.JourneySearch.Web.Controllers
{
    public class HomeController(IObiletSessionService obiletSessionService) : Controller
    {
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {

            var ipAddress = "165.114.41.21";

            var request = new GetSessionRequest(
                Type: 1,
                Connection: new GetSessionConnection(
                    IpAddress: ipAddress,
                    Port: "5117"),
                Browser: new GetSessionBrowser(
                    Name: "Chrome",
                    Version: "47.0.0.12"));

            await obiletSessionService.EnsureSessionAsync(
                HttpContext.Session,
                request,
                cancellationToken);


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
