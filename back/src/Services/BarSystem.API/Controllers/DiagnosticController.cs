using Microsoft.AspNetCore.Mvc;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace BarSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DiagnosticController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public DiagnosticController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("products-count")]
    public async Task<IActionResult> GetProductsCount()
    {
        try
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();

            // Contar todos os produtos (sem filtro)
            var sql = "SELECT COUNT(*) FROM \"Products\"";
            using var command = new NpgsqlCommand(sql, connection);
            var count = await command.ExecuteScalarAsync();

            // Ver tenants disponíveis
            var sqlTenants = "SELECT \"Id\", \"Name\" FROM \"Tenants\"";
            using var commandTenants = new NpgsqlCommand(sqlTenants, connection);
            using var reader = await commandTenants.ExecuteReaderAsync();
            
            var tenants = new List<object>();
            while (await reader.ReadAsync())
            {
                tenants.Add(new { Id = reader.GetGuid(0), Name = reader.GetString(1) });
            }

            return Ok(new 
            { 
                totalProducts = count,
                tenants = tenants
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
