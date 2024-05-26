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
            throw new UnAuthorizedAccessException("Neispravni podaci za prijavu");
        }

        if (!signInService.CheckPasswordHash(user.PasswordHash, request.Password))
        {
            throw new UnAuthorizedAccessException("Neispravn podaci za prijavu");
        }

        var newUser = await userRepository.UpdateLastLogin(user);
        var result = mapper.Map<LoggedInUserDto>(newUser);

        result.Token = signInService.GenerateJwToken(newUser);
        result.Role = (int)newUser.UserType;

        return result;
    }
}
