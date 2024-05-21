using AutoMapper;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using FER.InfSus.Time.Domain.Entities;

namespace FER.InfSus.Time.Application.Mappings;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, LoggedInUserDto>()
            .ForMember(t => t.Role, opt => opt.Ignore())
            .ForMember(t => t.Token, opt => opt.Ignore());

        CreateMap<User, UserDto>();
    }
}
