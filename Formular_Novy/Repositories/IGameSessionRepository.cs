using Formular_Novy.Models;

namespace Formular_Novy.Repositories;

/// <summary>
/// Rozhraní pro CRUD operace nad dětskou entitou GameSession.
/// Relace se vždy načítají pro konkrétní hru (gameId).
/// </summary>
public interface IGameSessionRepository
{
    Task<IEnumerable<GameSession>> GetByGameIdAsync(int gameId);
    Task<int> AddAsync(GameSession session);    // Vrátí nové ID
    Task UpdateAsync(GameSession session);
    Task DeleteAsync(int id);
}
