using BarSystem.Domain.Core.Entities;

namespace BarSystem.Core.Domain.Entities;

public class Table : TenantEntity
{
    public int Number { get; private set; }
    public int Capacity { get; private set; }
    public string Status { get; private set; } = "Free";
    public string? Location { get; private set; }

    protected Table() { }

    public Table(int number, int capacity, string? location = null)
    {
        Number = number;
        Capacity = capacity;
        Location = location;
    }

    public void Update(int number, int capacity, string? location)
    {
        Number = number;
        Capacity = capacity;
        Location = location;
        MarkAsUpdated();
    }

    public void SetStatus(string status)
    {
        Status = status;
        MarkAsUpdated();
    }
}
