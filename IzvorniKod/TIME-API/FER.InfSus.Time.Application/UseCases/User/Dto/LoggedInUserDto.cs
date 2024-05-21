namespace FER.InfSus.Time.Application.UseCases.User.Dto;

public class LoggedInUserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Username { get; set; } = null!;
    public int Role { get; set; }
    public string Token { get; set; } = null!;
}
