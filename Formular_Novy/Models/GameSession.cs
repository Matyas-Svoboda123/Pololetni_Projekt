namespace Formular_Novy.Models;

/// <summary>
/// Dětská entita — herní relace. Každá patří právě jedné hře (FK game_id).
/// Ukládá datum a počet odehraných hodin.
/// </summary>
public class GameSession
{
    public int Id { get; set; }
    public int GameId { get; set; }                         // FK → games.id
    public DateOnly SessionDate { get; set; }               // Datum relace
    public decimal HoursPlayed { get; set; }                // Počet hodin (> 0)
    public string? Notes { get; set; }                      // Volitelné poznámky
}
