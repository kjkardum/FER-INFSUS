using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using MediatR;
using System.Collections;
using System.Text.Json.Serialization;

namespace FER.InfSus.Time.Application.UseCases.TaskItem.Queries.GetAllByAssignedUser;

public class TaskItemGetAllByAssignedUserQuery: IRequest<ICollection<TaskItemSimpleDto>>
{
    [JsonIgnore] public Guid RequestorId { get; set; }
}
