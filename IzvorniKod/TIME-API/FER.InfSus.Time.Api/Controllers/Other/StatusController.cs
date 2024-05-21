using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FER.InfSus.Time.Api.Controllers.Other;

[ApiController]
public class StatusController: ControllerBase
{
    [HttpGet("health")]
    public ActionResult<string> HealthCheck() => Ok("Healthy");

    [Authorize]
    [HttpGet("authenticated")]
    public ActionResult<string> AuthCheck() => Ok("Authenticated");
}
