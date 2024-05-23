using ApiExceptions.Exceptions;
using AutoMapper;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Response;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.User.Queries.GetPaginated;

public class UserGetPaginatedQueryHandler(
    IUserRepository userRepository,
    IMapper mapper) :
        IRequestHandler<UserGetPaginatedQuery, PaginatedResponse<UserDto>>
{
    public async Task<PaginatedResponse<UserDto>> Handle(
        UserGetPaginatedQuery request,
        CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);
        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("You are not authorized to perform this action");
        }
        var users = await
            userRepository.GetPaginated(
                requestor.TenantId,
                request.Page,
                request.PageSize,
                request.OrderBy,
                request.FilterBy);
        var count = await userRepository.CountUsers();
        var userDtos = users.Select(mapper.Map<UserDto>).ToList();

        return new PaginatedResponse<UserDto>(request.Page, request.PageSize, count, userDtos);
    }
}
