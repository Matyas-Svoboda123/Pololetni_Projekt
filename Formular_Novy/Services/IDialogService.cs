using Formular_Novy.Models;

namespace Formular_Novy.Services;

/// <summary>
/// Rozhraní pro zobrazování dialogových oken.
/// ViewModel volá metody tohoto rozhraní místo přímého vytváření oken —
/// tím zůstává ViewModel testovatelný a nezávislý na UI.
/// </summary>
public interface IDialogService
{
    /// <summary>Otevře formulář pro přidání/úpravu hry. Vrátí true pokud uživatel uložil.</summary>
    Task<bool> ShowGameFormAsync(Game? game = null);

    /// <summary>Otevře formulář pro přidání/úpravu herní relace. Vrátí true pokud uložil.</summary>
    Task<bool> ShowSessionFormAsync(int gameId, GameSession? session = null);

    /// <summary>Zobrazí potvrzovací dialog (Ano/Ne). Vrátí true = Ano.</summary>
    Task<bool> ConfirmAsync(string message);
}
