namespace BarSystem.Core.Application.DTOs.Tables;

public class CreateTableDto
{
    public int Number { get; set; }
    public int Capacity { get; set; }
    public string? Location { get; set; }
}
