using System.Text;
using BarSystem.API.Middlewares;
using BarSystem.Core.Application.Services;
using BarSystem.Core.Application.Services.Auth;
using BarSystem.Core.Infra.Data;
using BarSystem.Domain.Core.Interfaces;
using BarSystem.Infra.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

// ============================================
// JWT CONFIGURATION
// ============================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ============================================
// DATABASE (DbContext com GroupId + TenantId)
// ============================================
builder.Services.AddScoped(sp =>
{
    var httpContext = sp.GetRequiredService<IHttpContextAccessor>().HttpContext;
    
    var groupId = httpContext?.Items["GroupId"] is Guid gid ? gid : Guid.Empty;
    var tenantId = httpContext?.Items["TenantId"] is Guid tid ? tid : (Guid?)null;

    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    var optionsBuilder = new DbContextOptionsBuilder<CoreDbContext>();
    optionsBuilder.UseNpgsql(connectionString);

    return new CoreDbContext(optionsBuilder.Options, groupId, tenantId);
});

// ============================================
// SERVICES
// ============================================
builder.Services.AddSingleton<IStorageService>(new LocalStorageService(uploadPath));
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// AppServices
builder.Services.AddScoped<GroupAppService>();
builder.Services.AddScoped<TenantAppService>();
builder.Services.AddScoped<UserAppService>();
builder.Services.AddScoped<CategoryAppService>();
builder.Services.AddScoped<ProductAppService>();
builder.Services.AddScoped<TableAppService>();
builder.Services.AddScoped<AuthAppService>();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Information);
});

// Controllers e Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "BarSystem API", Version = "v1" });
    
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddHttpContextAccessor();

// CORS para Angular
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Angular");
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TenantMiddleware>();

// Upload de Imagens
app.MapPost("/api/upload/image", async (IFormFile file, IStorageService storage) =>
{
    if (file == null || file.Length == 0)
        return Results.BadRequest(new { message = "Arquivo não enviado" });

    try
    {
        using var stream = file.OpenReadStream();
        var url = await storage.UploadImageAsync(stream, file.FileName, file.ContentType);
        return Results.Ok(new { url });
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(new { message = ex.Message });
    }
})
.DisableAntiforgery()
.Accepts<IFormFile>("multipart/form-data")
.Produces<object>(StatusCodes.Status200OK)
.WithName("UploadImage")
.WithTags("Upload");

app.MapGet("/api/health", () => Results.Ok(new { status = "OK", timestamp = DateTime.UtcNow }))
   .WithTags("Health");

app.MapControllers();

app.Run();
