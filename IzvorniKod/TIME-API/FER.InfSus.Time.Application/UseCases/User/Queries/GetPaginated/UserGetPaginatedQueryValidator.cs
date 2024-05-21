using FluentValidation;
using FER.InfSus.Time.Application.UseCases.User.Commands.GetPaginated;
using FER.InfSus.Time.Application.UseCases.User.Queries.GetPaginated;

namespace FER.InfSus.Time.Application.UseCases.Location.Queries.GetPaginated;

public class UserGetPaginatedQueryValidator: AbstractValidator<UserGetPaginatedQuery>
{
    public UserGetPaginatedQueryValidator()
    {
        RuleFor(t => t.Page)
            .GreaterThan(0);
        RuleFor(t => t.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}
