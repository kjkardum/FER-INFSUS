using FER.InfSus.Time.Api.Services;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.AddUser;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Create;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Delete;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.RemoveUser;
using FER.InfSus.Time.Application.UseCases.Taskboard.Commands.Update;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByTenant;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetAllByUserId;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FER.InfSus.Time.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskboardController(
    IMediator mediator,
    IAuthenticationService authenticationService): ControllerBase
{
    [HttpGet("{id:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskboardDetailedDto>> GetTaskboard(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var taskboard = await mediator.Send(
            new TaskboardGetByIdQuery
            {
                Id = id,
                RequestorId = (Guid)authenticationService.GetUserId()!
            },
            cancellationToken);

        return Ok(taskboard);
    }

    [HttpGet("assigned")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ICollection<TaskboardSimpleDto>>> GetAssignedTaskboards(
        CancellationToken cancellationToken = default)
    {
        var taskboards = await mediator.Send(
            new TaskboardGetAllByUserIdQuery
            {
                RequestorId = (Guid)authenticationService.GetUserId()!
            },
            cancellationToken);

        return Ok(taskboards);
    }

    [HttpGet("all")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ICollection<TaskboardSimpleDto>>> GetAllTaskboards(
        CancellationToken cancellationToken = default)
    {
        var taskboards = await mediator.Send(
            new TaskboardGetAllByTenantQuery
            {
                RequestorId = (Guid)authenticationService.GetUserId()!
            },
            cancellationToken);

        return Ok(taskboards);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TaskboardSimpleDto>> CreateTaskboard(
        TaskboardCreateCommand request,
        CancellationToken cancellationToken = default)
    {
        request.RequestorId = (Guid)authenticationService.GetUserId()!;

        var result = await mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetTaskboard), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateTaskboard(
        Guid id,
        TaskboardUpdateCommand request,
        CancellationToken cancellationToken = default)
    {
        request.Id = id;
        request.RequestorId = (Guid)authenticationService.GetUserId()!;

        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTaskboard(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(
            new TaskboardDeleteCommand
            {
                TaskboardId = id,
                RequestorId = (Guid)authenticationService.GetUserId()!
            },
            cancellationToken);

        return NoContent();
    }

    [HttpPost("assign")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> AssignTaskboard(
        TaskboardAddUserCommand request,
        CancellationToken cancellationToken = default)
    {
        request.RequestorId = (Guid)authenticationService.GetUserId()!;

        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

    [HttpPost("unassign")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UnassignTaskboard(
        TaskboardRemoveUserCommand request,
        CancellationToken cancellationToken = default)
    {
        request.RequestorId = (Guid)authenticationService.GetUserId()!;

        await mediator.Send(request, cancellationToken);
        return NoContent();
    }

}
