using Formular_Novy.Models;
using Npgsql;

namespace Formular_Novy.Repositories;

/// <summary>
/// Implementace CRUD pro tabulku games.
/// Používá parametrizované dotazy (prevence SQL injection).
/// JOIN s tabulkou platforms naplní navigační vlastnost Platform.
/// </summary>
public class GameRepository : IGameRepository
{
    private readonly string _connectionString;

    public GameRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Pomocná metoda — čte jeden řádek z DataReaderu a sestaví objekt Game
    private static Game MapGame(NpgsqlDataReader r) => new Game
    {
        Id = r.GetInt32(0),
        Title = r.GetString(1),
        Developer = r.IsDBNull(2) ? null : r.GetString(2),
        ReleaseYear = r.IsDBNull(3) ? null : r.GetInt32(3),
        PlatformId = r.GetInt32(4),
        Notes = r.IsDBNull(5) ? null : r.GetString(5),
        Platform = new Platform
        {
            Id = r.GetInt32(4),     // stejné jako PlatformId
            Name = r.GetString(6)   // jméno z JOINu
        }
    };

    public async Task<IEnumerable<Game>> GetAllAsync()
    {
        var games = new List<Game>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // JOIN s platforms — chceme zobrazit název platformy v seznamu
        const string sql = @"
            SELECT g.id, g.title, g.developer, g.release_year, g.platform_id, g.notes,
                   p.name AS platform_name
            FROM games g
            JOIN platforms p ON g.platform_id = p.id
            ORDER BY g.title";

        await using var cmd = new NpgsqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            games.Add(MapGame(reader));

        return games;
    }

    public async Task<Game?> GetByIdAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = @"
            SELECT g.id, g.title, g.developer, g.release_year, g.platform_id, g.notes,
                   p.name AS platform_name
            FROM games g
            JOIN platforms p ON g.platform_id = p.id
            WHERE g.id = @id";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", id);

        await using var reader = await cmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
            return MapGame(reader);

        return null;
    }

    public async Task<int> AddAsync(Game game)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // RETURNING id — PostgreSQL vrátí nově vygenerované ID
        const string sql = @"
            INSERT INTO games (title, developer, release_year, platform_id, notes)
            VALUES (@title, @developer, @releaseYear, @platformId, @notes)
            RETURNING id";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@title", game.Title);
        cmd.Parameters.AddWithValue("@developer", (object?)game.Developer ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@releaseYear", (object?)game.ReleaseYear ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@platformId", game.PlatformId);
        cmd.Parameters.AddWithValue("@notes", (object?)game.Notes ?? DBNull.Value);

        // ExecuteScalarAsync vrátí první sloupec prvního řádku = nové ID
        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    public async Task UpdateAsync(Game game)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = @"
            UPDATE games
            SET title = @title,
                developer = @developer,
                release_year = @releaseYear,
                platform_id = @platformId,
                notes = @notes
            WHERE id = @id";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", game.Id);
        cmd.Parameters.AddWithValue("@title", game.Title);
        cmd.Parameters.AddWithValue("@developer", (object?)game.Developer ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@releaseYear", (object?)game.ReleaseYear ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@platformId", game.PlatformId);
        cmd.Parameters.AddWithValue("@notes", (object?)game.Notes ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        // CASCADE v DB zajistí, že se smažou i game_sessions dané hry
        await using var cmd = new NpgsqlCommand(
            "DELETE FROM games WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        await cmd.ExecuteNonQueryAsync();
    }
}
