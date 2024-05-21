using Microsoft.AspNetCore.Mvc;
using FER.InfSus.Time.Api.ProblemDetails;

namespace FER.InfSus.Time.Api.Extensions;

internal class CustomProblemDetailsResponse : Microsoft.AspNetCore.Mvc.ProblemDetails
{
    public ErrorInfoDto[] Errors { get; set; } = null!;
}
