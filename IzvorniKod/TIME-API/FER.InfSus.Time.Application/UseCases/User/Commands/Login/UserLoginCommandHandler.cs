using ApiExceptions.Exceptions;
using AutoMapper;
using MediatR;
using FER.InfSus.Time.Application.Repositories;
using FER.InfSus.Time.Application.Services;
using FER.InfSus.Time.Application.UseCases.User.Dto;

namespace FER.InfSus.Time.Application.UseCases.User.Commands.Login;

public class UserLoginCommandHandler(
    IUserRepository userRepository,
    IMapper mapper,
    ISignInService signInService)
    : IRequestHandler<UserLoginCommand, LoggedInUserDto>
{
    public async Task<LoggedInUserDto> Handle(UserLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmail(request.NormalizedEmail);
        if (user == null)
        {
            throw new EntityNotFoundException("User with this email doesn't exist");
        }

        if (!signInService.CheckPasswordHash(user.PasswordHash, request.Password))
        {
            throw new UnAuthorizedAccessException("Invalid password");
        }

        var newUser = await userRepository.UpdateLastLogin(user);
        var result = mapper.Map<LoggedInUserDto>(newUser);

        result.Token = signInService.GenerateJwToken(newUser);
        result.Role = (int)newUser.UserType;

        return result;
    }
}
