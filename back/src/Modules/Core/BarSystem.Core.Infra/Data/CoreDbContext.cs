using BarSystem.Core.Domain.Entities;
using BarSystem.Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarSystem.Core.Infra.Data;

public class CoreDbContext : DbContext
{
    private readonly Guid _currentGroupId;
    private readonly Guid? _currentTenantId;

    public DbSet<Group> Groups => Set<Group>();
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Table> Tables => Set<Table>();

    public CoreDbContext(
        DbContextOptions<CoreDbContext> options, 
        Guid currentGroupId,
        Guid? currentTenantId) : base(options)
    {
        _currentGroupId = currentGroupId;
        _currentTenantId = currentTenantId;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var currentGroupId = _currentGroupId;
        var currentTenantId = _currentTenantId;

        // ===== GROUP =====
        modelBuilder.Entity<Group>(e =>
        {
            e.ToTable("Groups");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.ContactEmail).HasMaxLength(200);
            e.Property(x => x.ContactPhone).HasMaxLength(20);
        });

        // ===== TENANT =====
        modelBuilder.Entity<Tenant>(e =>
        {
            e.ToTable("Tenants");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Cnpj).HasMaxLength(18);
            
            e.HasOne<Group>()
                .WithMany(g => g.Tenants)
                .HasForeignKey(t => t.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
            
            e.HasIndex(x => new { x.GroupId, x.Name });
            
            if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
            {
                e.HasQueryFilter(t => t.GroupId == currentGroupId && t.Id == currentTenantId.Value);
            }
            else
            {
                e.HasQueryFilter(t => t.GroupId == currentGroupId);
            }
        });

        // ===== USER =====
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Email).IsRequired().HasMaxLength(200);
            e.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);
            e.Property(x => x.Role).IsRequired().HasMaxLength(50);
            e.Property(x => x.TenantId).IsRequired(false);
            
            e.HasOne<Group>()
                .WithMany(g => g.Users)
                .HasForeignKey(u => u.GroupId)
                .OnDelete(DeleteBehavior.Restrict);
            
            e.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(u => u.TenantId)
                .OnDelete(DeleteBehavior.Restrict);
            
            e.HasIndex(x => new { x.GroupId, x.Email }).IsUnique();
            
            e.HasQueryFilter(u => u.GroupId == currentGroupId);
        });

        // ===== CATEGORY =====
        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Categories");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(100);
            e.Property(x => x.Description).HasMaxLength(500);
            e.HasIndex(x => new { x.TenantId, x.Name });
            
            if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
            {
                e.HasQueryFilter(c => c.TenantId == currentTenantId.Value);
            }
            else
            {
                e.HasQueryFilter(c => true);
            }
        });

        // ===== PRODUCT =====
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.Property(x => x.ImageUrl).HasMaxLength(500);
            
            e.HasOne(p => p.Category)
                .WithMany()
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
            e.HasIndex(x => new { x.TenantId, x.CategoryId });
            
            if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
            {
                e.HasQueryFilter(p => p.TenantId == currentTenantId.Value);
            }
            else
            {
                e.HasQueryFilter(p => true);
            }
        });

        // ===== TABLE =====
        modelBuilder.Entity<Table>(e =>
        {
            e.ToTable("Tables");
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).IsRequired().HasMaxLength(20);
            e.Property(x => x.Location).HasMaxLength(100);
            e.HasIndex(x => new { x.TenantId, x.Number }).IsUnique();
            
            if (currentTenantId.HasValue && currentTenantId.Value != Guid.Empty)
            {
                e.HasQueryFilter(t => t.TenantId == currentTenantId.Value);
            }
            else
            {
                e.HasQueryFilter(t => true);
            }
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity is GroupEntity groupEntity && groupEntity.GroupId == Guid.Empty)
                {
                    groupEntity.SetGroupId(_currentGroupId);
                }
                
                if (entry.Entity is TenantEntity tenantEntity && tenantEntity.TenantId == Guid.Empty)
                {
                    tenantEntity.SetTenantId(_currentTenantId ?? Guid.Empty);
                }
            }
        }
        return base.SaveChangesAsync(ct);
    }
}
