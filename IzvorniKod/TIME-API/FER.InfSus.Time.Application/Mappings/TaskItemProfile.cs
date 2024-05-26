using AutoMapper;
using FER.InfSus.Time.Application.UseCases.TaskItem.Dtos;
using FER.InfSus.Time.Domain.Entities;

namespace FER.InfSus.Time.Application.Mappings;

public class TaskItemProfile : Profile
{
    public TaskItemProfile()
    {
        CreateMap<TaskItem, TaskItemSimpleDto>();
        CreateMap<TaskItem, TaskItemDetailedDto>();
        CreateMap<TaskItem, TaskItemForTasklistDto>()
            .ForMember(
                t => t.TaskboardName,
                opt => opt.MapFrom(t => t.Taskboard!.Name));
        CreateMap<TaskItemHistoryLog, TaskItemHistoryLogDto>();
    }
}
