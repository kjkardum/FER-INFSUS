using ApiExceptions.Exceptions;
using AutoMapper;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Services;
using FER.InfSus.Time.Application.UseCases.User.Dto;
using FER.InfSus.Time.Domain.Enums;
using MediatR;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Create;

public class UserRegistrationCommandHandler(
    IUserRepository userRepository,
    ISignInService signInService)
    : IRequestHandler<UserRegistrationCommand>
{
    public async Task Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
    {
        var requestor = await userRepository.GetByUserId(request.RequestorId);

        if (requestor?.UserType != UserType.ADMIN)
        {
            throw new ForbiddenAccessException("Only admins can create users");
        }

        if (await userRepository.DoesUserExist(request.NormalizedEmail))
        {
            throw new EntityAlreadyExistsException("User with this email already exists");
        }

        var passwordHash = signInService.HashPassword(request.Password);
        if (passwordHash == null)
        {
            throw new BadRequestException("Invalid password");
        }

        var user = new Domain.Entities.User {
            TenantId = requestor.TenantId,
            Email = request.NormalizedEmail,
            PasswordHash = passwordHash,
            FirstName = request.FirstName,
            LastName = request.LastName,
            LastLogin = DateTime.UtcNow,
            UserType = UserType.USER
        };

        await userRepository.Create(user);
    }
}
