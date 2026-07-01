namespace BarSystem.Domain.Core.Entities;

public abstract class GroupEntity : Entity
{
    public Guid GroupId { get; protected set; }

    protected GroupEntity() : base() { }

    public void SetGroupId(Guid groupId)
    {
        GroupId = groupId;
    }
}
