namespace BarSystem.Core.Application.DTOs.Auth;

public class RegisterDto
{
    public string GroupName { get; set; } = string.Empty;
    public string? GroupDescription { get; set; }
    public string? GroupContactEmail { get; set; }
    public string? GroupContactPhone { get; set; }
    public string FirstBarName { get; set; } = string.Empty;
    public string? FirstBarCnpj { get; set; }
    public string AdminName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
}
