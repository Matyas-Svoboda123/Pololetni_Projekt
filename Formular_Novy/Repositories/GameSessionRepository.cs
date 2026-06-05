using Formular_Novy.Models;
using Npgsql;

namespace Formular_Novy.Repositories;

/// <summary>
/// Implementace CRUD pro tabulku game_sessions (dětská entita).
/// Veškerý SQL kód patří sem — ViewModel o SQL neví.
/// </summary>
public class GameSessionRepository : IGameSessionRepository
{
    private readonly string _connectionString;

    public GameSessionRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    // Mapovací pomocná metoda
    private static GameSession MapSession(NpgsqlDataReader r) => new GameSession
    {
        Id = r.GetInt32(0),
        GameId = r.GetInt32(1),
        // PostgreSQL DATE → DateOnly (Npgsql 6+ podporuje nativní konverzi)
        SessionDate = DateOnly.FromDateTime(r.GetDateTime(2)),
        HoursPlayed = r.GetDecimal(3),
        Notes = r.IsDBNull(4) ? null : r.GetString(4)
    };

    public async Task<IEnumerable<GameSession>> GetByGameIdAsync(int gameId)
    {
        var sessions = new List<GameSession>();

        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = @"
            SELECT id, game_id, session_date, hours_played, notes
            FROM game_sessions
            WHERE game_id = @gameId
            ORDER BY session_date DESC";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@gameId", gameId);

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            sessions.Add(MapSession(reader));

        return sessions;
    }

    public async Task<int> AddAsync(GameSession session)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = @"
            INSERT INTO game_sessions (game_id, session_date, hours_played, notes)
            VALUES (@gameId, @sessionDate, @hoursPlayed, @notes)
            RETURNING id";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@gameId", session.GameId);
        // DateOnly → předáme jako DateTime (Npgsql to zvládne přiřadit k DATE sloupci)
        cmd.Parameters.AddWithValue("@sessionDate", session.SessionDate.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@hoursPlayed", session.HoursPlayed);
        cmd.Parameters.AddWithValue("@notes", (object?)session.Notes ?? DBNull.Value);

        return (int)(await cmd.ExecuteScalarAsync())!;
    }

    public async Task UpdateAsync(GameSession session)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        const string sql = @"
            UPDATE game_sessions
            SET session_date = @sessionDate,
                hours_played = @hoursPlayed,
                notes = @notes
            WHERE id = @id";

        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@id", session.Id);
        cmd.Parameters.AddWithValue("@sessionDate", session.SessionDate.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@hoursPlayed", session.HoursPlayed);
        cmd.Parameters.AddWithValue("@notes", (object?)session.Notes ?? DBNull.Value);

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await using var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new NpgsqlCommand(
            "DELETE FROM game_sessions WHERE id = @id", conn);
        cmd.Parameters.AddWithValue("@id", id);

        await cmd.ExecuteNonQueryAsync();
    }
}
