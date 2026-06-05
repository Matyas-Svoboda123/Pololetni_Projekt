using Formular_Novy.Models;

namespace Formular_Novy.Repositories;

/// <summary>
/// Rozhraní pro přístup k číselníku platforem.
/// Číselník se pouze čte — naplní ho seed.sql.
/// </summary>
public interface IPlatformRepository
{
    Task<IEnumerable<Platform>> GetAllAsync();
}
