namespace FER.InfSus.Time.Api.ProblemDetails;

internal record ErrorInfoDto
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = null!;
}
