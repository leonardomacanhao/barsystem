namespace BarSystem.Core.Application.DTOs.Groups;

public class UpdateGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
}
