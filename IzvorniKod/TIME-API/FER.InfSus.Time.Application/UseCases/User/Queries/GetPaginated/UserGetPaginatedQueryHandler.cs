using AutoMapper;
using MediatR;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Response;
using FER.InfSus.Time.Application.UseCases.Location.Queries.GetPaginated;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using FER.InfSus.Time.Application.UseCases.User.Queries.GetPaginated;
using FER.InfSus.Time.Domain.Enums;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.GetPaginated;

public class UserGetPaginatedQueryHandler:
    IRequestHandler<UserGetPaginatedQuery, PaginatedResponse<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserGetPaginatedQueryHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<PaginatedResponse<UserDto>> Handle(
        UserGetPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var users = await
            _userRepository.GetPaginated(request.Page, request.PageSize, request.OrderBy, request.FilterBy);
        var count = await _userRepository.CountUsers();
        var userDtos = users.Select(t => _mapper.Map<UserDto>(t)).ToList();

        return new PaginatedResponse<UserDto>(request.Page, request.PageSize, count, userDtos);
    }
}
