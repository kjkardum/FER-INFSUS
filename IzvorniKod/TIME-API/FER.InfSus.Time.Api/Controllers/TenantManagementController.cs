using FER.InfSus.Time.Api.Services;
using FER.InfSus.Time.Application.Request;
using FER.InfSus.Time.Application.Response;
using FER.InfSus.Time.Application.UseCases.User.Commands.Create;
using FER.InfSus.Time.Application.UseCases.User.Commands.Delete;
using FER.InfSus.Time.Application.UseCases.User.Commands.Update;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using FER.InfSus.Time.Application.UseCases.User.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FER.InfSus.Time.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantManagementController(
    IMediator mediator,
    IAuthenticationService authenticationService) : ControllerBase
{
    [HttpPost("createUser")]
    public async Task<ActionResult> CreateUser(
        UserRegistrationCommand request,
        CancellationToken cancellationToken = default)
    {
        request.RequestorId = (Guid)authenticationService.GetUserId()!;

        await mediator.Send(request, cancellationToken);
        return Ok();
    }

    [HttpGet("getUsers")]
    public async Task<ActionResult<PaginatedResponse<UserDto>>> GetPaginatedUsers(
        [FromQuery] PaginatedRequest request,
        CancellationToken cancellationToken = default)
    {
        var paginatedUsers = await mediator.Send(
            new UserGetPaginatedQuery
                {
                    Page = request.Page,
                    PageSize = request.PageSize,
                    FilterBy = request.FilterBy,
                    OrderBy = request.OrderBy,
                    RequestorId = (Guid)authenticationService.GetUserId()!
                },
            cancellationToken);

        return Ok(paginatedUsers);
    }

    [HttpPut("updateUser/{id:guid}")]
    public async Task<ActionResult> UpdateUser(
        Guid id,
        UserUpdateCommand request,
        CancellationToken cancellationToken = default)
    {
        request.RequestorId = (Guid)authenticationService.GetUserId()!;
        request.Id = id;

        await mediator.Send(request, cancellationToken);
        return Ok();
    }

    [HttpDelete("deleteUser/{id:guid}")]
    public async Task<ActionResult> DeleteUser(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var request = new UserDeleteCommand
        {
            RequestorId = (Guid)authenticationService.GetUserId()!,
            Id = id
        };

        await mediator.Send(request, cancellationToken);
        return Ok();
    }
}
