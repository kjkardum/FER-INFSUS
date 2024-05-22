using System.Security.Claims;

namespace FER.InfSus.Time.Api.Services;

public class AuthenticationService(IHttpContextAccessor httpContextAccessor) : IAuthenticationService
{
    public Guid? GetUserId() =>
        httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated == true
            ? Guid.Parse(httpContextAccessor.HttpContext.User.FindFirstValue("uid")!)
            : null;
}
