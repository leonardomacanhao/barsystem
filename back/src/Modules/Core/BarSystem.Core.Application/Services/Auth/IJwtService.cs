using BarSystem.Core.Domain.Entities;

namespace BarSystem.Core.Application.Services.Auth;

public interface IJwtService
{
    string GenerateToken(User user, Group group, Tenant? tenant = null);
}
