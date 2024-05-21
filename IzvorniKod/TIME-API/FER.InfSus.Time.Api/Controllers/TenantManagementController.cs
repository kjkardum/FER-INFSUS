using FER.InfSus.Time.Api.Services;
using FER.InfSus.Time.Application.UseCases.User.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FER.InfSus.Time.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantManagementController(
    IMediator mediator,
    IAuthenticationService authenticationService): ControllerBase
{
    [HttpPost("createUser")]
    [Authorize]
    public async Task<ActionResult> CreateUser(
        UserRegistrationCommand request,
        CancellationToken cancellationToken = default)
    {
        request.RequestorId = (Guid)authenticationService.GetUserId()!;

        await mediator.Send(request, cancellationToken);
        return Ok();
    }
}
