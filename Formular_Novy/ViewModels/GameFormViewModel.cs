using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Formular_Novy.Models;
using Formular_Novy.Repositories;

namespace Formular_Novy.ViewModels;

/// <summary>
/// View 3 — formulář pro vytvoření / úpravu hry.
/// Zobrazí se jako modální dialog (GameFormView okno).
/// ComboBox je napojen na seznam platforem načtený z DB.
/// </summary>
public partial class GameFormViewModel : ViewModelBase
{
    private readonly IGameRepository _gameRepo;
    private readonly IPlatformRepository _platformRepo;

    private int? _editingId; // null = nová hra, jinak = editace

    // ── Formulářová pole ──────────────────────────────────────────

    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string _developer = string.Empty;
    [ObservableProperty] private string _releaseYear = string.Empty; // Jako string kvůli validaci
    [ObservableProperty] private Platform? _selectedPlatform;
    [ObservableProperty] private string _notes = string.Empty;
    [ObservableProperty] private ObservableCollection<Platform> _platforms = new();

    // Chybová zpráva validace — zobrazí se pod formulářem
    [ObservableProperty] private string _validationError = string.Empty;

    // Nadpis dialogu — "Přidat hru" nebo "Upravit hru"
    [ObservableProperty] private string _windowTitle = "Přidat hru";

    /// <summary>True po úspěšném uložení — DialogService to zjistí po zavření okna.</summary>
    public bool WasSaved { get; private set; }

    /// <summary>Vyvolání tohoto eventu zavře dialog (code-behind zaregistruje Close).</summary>
    public event Action? RequestClose;

    public GameFormViewModel(IGameRepository gameRepo, IPlatformRepository platformRepo)
    {
        _gameRepo = gameRepo;
        _platformRepo = platformRepo;
    }

    /// <summary>Inicializuje formulář — načte platformy a případně předvyplní hodnoty editovaného záznamu.</summary>
    public async Task InitializeAsync(Game? game = null)
    {
        var platforms = await _platformRepo.GetAllAsync();
        Platforms = new ObservableCollection<Platform>(platforms);

        if (game != null)
        {
            // Editujeme existující hru — předvyplníme pole
            _editingId = game.Id;
            WindowTitle = "Upravit hru";
            Title = game.Title;
            Developer = game.Developer ?? string.Empty;
            ReleaseYear = game.ReleaseYear?.ToString() ?? string.Empty;
            Notes = game.Notes ?? string.Empty;
            // Najdeme odpovídající objekt platformy v načteném seznamu (aby ComboBox fungoval)
            SelectedPlatform = Platforms.FirstOrDefault(p => p.Id == game.PlatformId);
        }
        else
        {
            _editingId = null;
            WindowTitle = "Přidat hru";
        }
    }

    // ── Příkazy ────────────────────────────────────────────────────

    [RelayCommand]
    private async Task Save()
    {
        // Validace povinných polí
        ValidationError = string.Empty;

        if (string.IsNullOrWhiteSpace(Title))
        {
            ValidationError = "Název hry je povinný.";
            return;
        }

        if (SelectedPlatform == null)
        {
            ValidationError = "Vyberte platformu.";
            return;
        }

        // Validace roku — nepovinný, ale pokud zadán, musí být číslo ve smysluplném rozsahu
        int? year = null;
        if (!string.IsNullOrWhiteSpace(ReleaseYear))
        {
            if (!int.TryParse(ReleaseYear.Trim(), out var y) || y < 1950 || y > 2100)
            {
                ValidationError = "Rok vydání musí být číslo mezi 1950 a 2100.";
                return;
            }
            year = y;
        }

        var game = new Game
        {
            Id = _editingId ?? 0,
            Title = Title.Trim(),
            Developer = string.IsNullOrWhiteSpace(Developer) ? null : Developer.Trim(),
            ReleaseYear = year,
            PlatformId = SelectedPlatform.Id,
            Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
        };

        if (_editingId.HasValue)
            await _gameRepo.UpdateAsync(game);
        else
            await _gameRepo.AddAsync(game);

        WasSaved = true;
        RequestClose?.Invoke(); // Zavřeme dialog
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke(); // Zavřeme bez uložení
    }
}
