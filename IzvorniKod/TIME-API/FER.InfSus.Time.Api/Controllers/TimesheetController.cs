using FER.InfSus.Time.Api.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FER.InfSus.Time.Api.Controllers;

public class TimesheetController(IMediator mediator, IAuthenticationService authenticationService): ControllerBase
{
}
