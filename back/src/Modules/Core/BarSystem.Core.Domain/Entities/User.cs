using BarSystem.Domain.Core.Entities;

namespace BarSystem.Core.Domain.Entities;

public class User : GroupEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = string.Empty; // GroupAdmin, BarManager, Waiter, Cashier
    public Guid? TenantId { get; private set; } // NULL = acesso a todos os bares do grupo
    public bool IsActive { get; private set; } = true;

    protected User() { }

    public User(string name, string email, string passwordHash, string role, Guid? tenantId = null)
    {
        Name = name;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        TenantId = tenantId;
    }

    public void Update(string name, string email, string role, Guid? tenantId)
    {
        Name = name;
        Email = email;
        Role = role;
        TenantId = tenantId;
        MarkAsUpdated();
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        MarkAsUpdated();
    }

    public bool HasAccessToTenant(Guid tenantId)
    {
        // Se TenantId é null, tem acesso a todos os bares do grupo
        if (TenantId == null) return true;
        // Senão, só tem acesso ao seu bar específico
        return TenantId.Value == tenantId;
    }
}
