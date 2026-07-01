namespace BarSystem.API.Middlewares;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var groupIdClaim = context.User.FindFirst("GroupId")?.Value;
            if (Guid.TryParse(groupIdClaim, out var groupId))
            {
                context.Items["GroupId"] = groupId;
            }

            var tenantIdClaim = context.User.FindFirst("TenantId")?.Value;
            
            if (!string.IsNullOrEmpty(tenantIdClaim) && 
                Guid.TryParse(tenantIdClaim, out var tenantId) && 
                tenantId != Guid.Empty)
            {
                context.Items["TenantId"] = tenantId;
            }
        }

        await _next(context);
    }
}
