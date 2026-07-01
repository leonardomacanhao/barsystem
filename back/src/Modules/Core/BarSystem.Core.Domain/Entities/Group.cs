using BarSystem.Domain.Core.Entities;

namespace BarSystem.Core.Domain.Entities;

public class Group : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? ContactEmail { get; private set; }
    public string? ContactPhone { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation
    public ICollection<Tenant> Tenants { get; private set; } = new List<Tenant>();
    public ICollection<User> Users { get; private set; } = new List<User>();

    protected Group() { }

    public Group(string name, string? description = null, string? contactEmail = null, string? contactPhone = null)
    {
        Name = name;
        Description = description;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
    }

    public void Update(string name, string? description, string? contactEmail, string? contactPhone)
    {
        Name = name;
        Description = description;
        ContactEmail = contactEmail;
        ContactPhone = contactPhone;
        MarkAsUpdated();
    }
}
