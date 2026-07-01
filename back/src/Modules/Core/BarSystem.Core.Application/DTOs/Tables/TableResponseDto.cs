namespace BarSystem.Core.Application.DTOs.Tables;

public class TableResponseDto
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
