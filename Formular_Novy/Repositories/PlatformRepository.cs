using Formular_Novy.Models;
using Npgsql;

namespace Formular_Novy.Repositories;

/// <summary>
/// Čte číselník platforem z databáze.
/// Implementuje repository pattern — veškerý SQL je zde, ne ve ViewModelu.
/// </summary>
public class PlatformRepository : IPlatformRepository
{
    private readonly string _connectionString;

    public PlatformRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IEnumerable<Platform>> GetAllAsync()
    {
        var platforms = new List<Platform>();

        // Otevřeme nové připojení k PostgreSQL (using zajistí automatické zavření)
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "SELECT id, name FROM platforms ORDER BY name", conn);

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            platforms.Add(new Platform
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1)
            });
        }

        return platforms;
    }
}
