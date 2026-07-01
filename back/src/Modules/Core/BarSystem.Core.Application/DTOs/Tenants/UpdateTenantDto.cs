namespace BarSystem.Core.Application.DTOs.Tenants;

public class UpdateTenantDto
{
    public string Name { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
}
