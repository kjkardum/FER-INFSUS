using FER.InfSus.Time.Application.Request;
using FER.InfSus.Time.Application.Response;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.User.Queries.GetPaginated;

public record UserGetPaginatedQuery: PaginatedRequest, IRequest<PaginatedResponse<UserDto>>
{
    public Guid? UserId { get; set; }
}
