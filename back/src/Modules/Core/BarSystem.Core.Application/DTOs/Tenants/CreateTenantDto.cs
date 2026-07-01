namespace BarSystem.Core.Application.DTOs.Tenants;

public class CreateTenantDto
{
    public string Name { get; set; } = string.Empty;
    public string? Cnpj { get; set; }
}
