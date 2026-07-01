namespace BarSystem.Domain.Core.Entities;

public abstract class TenantEntity : Entity
{
    public Guid TenantId { get; protected set; }

    protected TenantEntity() : base() { }

    public void SetTenantId(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
