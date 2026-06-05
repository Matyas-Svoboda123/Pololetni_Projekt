using Formular_Novy.Models;

namespace Formular_Novy.Repositories;

/// <summary>
/// Rozhraní pro CRUD operace nad hlavní entitou Game.
/// </summary>
public interface IGameRepository
{
    Task<IEnumerable<Game>> GetAllAsync();
    Task<Game?> GetByIdAsync(int id);
    Task<int> AddAsync(Game game);      // Vrátí nové ID
    Task UpdateAsync(Game game);
    Task DeleteAsync(int id);
}
