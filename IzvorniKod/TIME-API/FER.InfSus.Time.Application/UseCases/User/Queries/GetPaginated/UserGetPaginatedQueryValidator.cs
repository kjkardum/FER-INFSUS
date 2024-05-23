using FluentValidation;

namespace FER.InfSus.Time.Application.UseCases.User.Queries.GetPaginated;

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
