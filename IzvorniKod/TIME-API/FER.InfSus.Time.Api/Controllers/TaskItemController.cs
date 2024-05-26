using FER.InfSus.Time.Api.Services;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Application.UseCases.Taskboard.Queries.GetById;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Assign;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeDescription;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeState;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.ChangeTaskboard;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Create;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Delete;
using FER.InfSus.Time.Application.UseCases.TaskItem.Commands.Rename;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Application.UseCases.TaskItem.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace FER.InfSus.Time.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskItemController(
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
    public async Task<ActionResult<TaskItemDetailedDto>> GetTaskItem(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var taskboard = await mediator.Send(
            new TaskItemGetByIdQuery
            {
                Id = id,
                RequestorId = (Guid)authenticationService.GetUserId()!
            },
            cancellationToken);

        return Ok(taskboard);
    }

    [HttpPost]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TaskItemSimpleDto>> CreateTaskItem(
        TaskItemCreateCommand command,
        CancellationToken cancellationToken = default)
    {
        command.RequestorId = (Guid)authenticationService.GetUserId()!;
        var taskItem = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(
            nameof(GetTaskItem),
            new { id = taskItem.Id },
            taskItem);
    }

    [HttpPost("{id:guid}/assign")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> AssignTaskItem(
        Guid id,
        TaskItemAssignCommand command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        command.RequestorId = (Guid)authenticationService.GetUserId()!;
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/rename")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RenameTaskItem(
        Guid id,
        TaskItemRenameCommand command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        command.RequestorId = (Guid)authenticationService.GetUserId()!;
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/changeDescription")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangeTaskItemDescription(
        Guid id,
        TaskItemChangeDescriptionCommand command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        command.RequestorId = (Guid)authenticationService.GetUserId()!;
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/changeState")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> SetTaskItemState(
        Guid id,
        TaskItemChangeStateCommand command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        command.RequestorId = (Guid)authenticationService.GetUserId()!;
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpPost("{id:guid}/move")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> MoveTaskItem(
        Guid id,
        TaskItemChangeTaskboardCommand command,
        CancellationToken cancellationToken = default)
    {
        command.Id = id;
        command.RequestorId = (Guid)authenticationService.GetUserId()!;
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Microsoft.AspNetCore.Mvc.ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTaskItem(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        await mediator.Send(
            new TaskItemDeleteCommand
            {
                Id = id,
                RequestorId = (Guid)authenticationService.GetUserId()!
            },
            cancellationToken);

        return NoContent();
    }
}
