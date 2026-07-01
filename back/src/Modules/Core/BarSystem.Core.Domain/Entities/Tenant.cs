using BarSystem.Domain.Core.Entities;

namespace BarSystem.Core.Domain.Entities;

public class Tenant : GroupEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Cnpj { get; private set; }
    public bool IsActive { get; private set; } = true;

    protected Tenant() { }

    public Tenant(string name, string? cnpj = null)
    {
        Name = name;
        Cnpj = cnpj;
    }

    public void Update(string name, string? cnpj)
    {
        Name = name;
        Cnpj = cnpj;
        MarkAsUpdated();
    }
}
