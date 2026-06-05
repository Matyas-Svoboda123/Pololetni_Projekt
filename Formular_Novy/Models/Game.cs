namespace Formular_Novy.Models;

/// <summary>
/// Hlavní entita — hra. Má cizí klíč na Platform (číselník).
/// Jedna hra může mít mnoho herních relací (1:N na GameSession).
/// </summary>
public class Game
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;       // Povinný název
    public string? Developer { get; set; }                  // Volitelný vývojář
    public int? ReleaseYear { get; set; }                   // Volitelný rok vydání
    public int PlatformId { get; set; }                     // FK → platforms.id
    public Platform? Platform { get; set; }                 // Navigační vlastnost (JOIN)
    public string? Notes { get; set; }                      // Volitelné poznámky
}
