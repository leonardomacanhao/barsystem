using BarSystem.Domain.Core.Entities;

namespace BarSystem.Core.Domain.Entities;

public class Product : TenantEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public Guid CategoryId { get; private set; }
    public string? ImageUrl { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    // Propriedade de navegação
    public Category? Category { get; private set; }

    protected Product() { }

    public Product(string name, string? description, decimal price, Guid categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
    }

    public void Update(string name, string? description, decimal price, Guid categoryId)
    {
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        MarkAsUpdated();
    }

    public void SetImage(string? imageUrl)
    {
        ImageUrl = imageUrl;
        MarkAsUpdated();
    }
}
