using BarSystem.Domain.Core.Entities;

namespace BarSystem.Core.Domain.Entities;

public class Category : TenantEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public bool IsActive { get; private set; } = true;

    protected Category() { }

    public Category(string name, string? description = null)
    {
        Name = name;
        Description = description;
    }

    public void Update(string name, string? description)
    {
        Name = name;
        Description = description;
        MarkAsUpdated();
    }
}
