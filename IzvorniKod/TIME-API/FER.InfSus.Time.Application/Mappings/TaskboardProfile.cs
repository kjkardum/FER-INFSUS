using AutoMapper;
using FER.InfSus.Time.Application.UseCases.Taskboard.Dtos;
using FER.InfSus.Time.Domain;
using FER.InfSus.Time.Domain.Entities;

namespace FER.InfSus.Time.Application.Mappings;

public class TaskboardProfile: Profile
{
    public TaskboardProfile()
    {
        CreateMap<Taskboard, TaskboardDetailedDto>()
            .ForMember(
                t => t.TaskboardUsers,
                opt => opt.MapFrom(d
                    => (d.TaskboardUsers ?? new List<UserTaskboardAssociation>()).Select(tu
                        => tu.User)));
        CreateMap<Taskboard, TaskboardSimpleDto>()
            .ForMember(
                t => t.TaskboardUsers,
                opt => opt.MapFrom(d
                    => (d.TaskboardUsers ?? new List<UserTaskboardAssociation>()).Select(tu
                        => tu.User)));
    }
}
