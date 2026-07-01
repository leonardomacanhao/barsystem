using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BarSystem.Core.Infra.Data;

public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
    public CoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
        optionsBuilder.UseNpgsql("Host=postgres-prod-123456789.us-east-1.rds.amazonaws.com;Port=5432;Database=BarSystem_Dev;Username=postgres;Password=postgres123");
        
        return new CoreDbContext(optionsBuilder.Options, Guid.Empty, null);
    }
}
