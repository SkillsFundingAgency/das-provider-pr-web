using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SFA.DAS.Provider.PR.Web.Infrastructure;
using SFA.DAS.Provider.PR.Web.Infrastructure.Configuration;
using SFA.DAS.Provider.PR.Web.Models;

namespace SFA.DAS.Provider.PR.Web.Controllers;

[Route("[controller]")]
public class ErrorController(ILogger<ErrorController> _logger, IOptions<ApplicationSettings> _applicationSettings) : Controller
{
    [AllowAnonymous]
    [Route("{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        ErrorViewModel errorViewModel = new(Url.RouteUrl(RouteNames.Home)!, _applicationSettings.Value.DfESignInServiceHelpUrl);

        return statusCode switch
        {
            403 => View("AccessDenied", errorViewModel),
            404 => View("PageNotFound"),
            _ => View("ErrorInService", errorViewModel)
        };
    }

    [AllowAnonymous]
    public IActionResult ErrorInService()
    {
        var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        ErrorViewModel errorViewModel = new(Url.RouteUrl(RouteNames.Home)!, _applicationSettings.Value.DfESignInServiceHelpUrl);

        if (User.Identity!.IsAuthenticated)
        {
            _logger.LogError(feature!.Error, "Unexpected error occurred during request to path: {Path} by user: {User}", feature.Path, User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
        else
        {
            _logger.LogError(feature!.Error, "Unexpected error occurred during request to {Path}", feature.Path);
        }
        return View(errorViewModel);
    }
}
