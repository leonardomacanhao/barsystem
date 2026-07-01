namespace BarSystem.Core.Application.DTOs.Tenants;

public class TenantResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
    public bool IsActive { get; set; }
    public Guid GroupId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
