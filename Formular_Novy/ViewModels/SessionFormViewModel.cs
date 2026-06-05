using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Formular_Novy.Models;
using Formular_Novy.Repositories;

namespace Formular_Novy.ViewModels;

/// <summary>
/// ViewModel pro formulář herní relace (přidání / úprava).
/// Zobrazí se jako modální dialog SessionFormView.
/// </summary>
public partial class SessionFormViewModel : ViewModelBase
{
    private readonly IGameSessionRepository _sessionRepo;

    private int _gameId;      // Ke které hře relace patří
    private int? _editingId;  // null = nová relace, jinak = editace

    // ── Formulářová pole ──────────────────────────────────────────

    // DateTimeOffset je typ který DatePicker v Avalonii používá
    [ObservableProperty]
    private DateTimeOffset _sessionDate = DateTimeOffset.Now;

    [ObservableProperty]
    private decimal _hoursPlayed = 1.0m;

    [ObservableProperty]
    private string _notes = string.Empty;

    [ObservableProperty]
    private string _validationError = string.Empty;

    [ObservableProperty]
    private string _windowTitle = "Přidat herní relaci";

    public bool WasSaved { get; private set; }
    public event Action? RequestClose;

    public SessionFormViewModel(IGameSessionRepository sessionRepo)
    {
        _sessionRepo = sessionRepo;
    }

    /// <summary>Inicializuje formulář — předvyplní hodnoty editované relace, pokud existuje.</summary>
    public Task InitializeAsync(int gameId, GameSession? session = null)
    {
        _gameId = gameId;

        if (session != null)
        {
            _editingId = session.Id;
            WindowTitle = "Upravit herní relaci";
            // Převedeme DateOnly → DateTimeOffset pro DatePicker
            SessionDate = new DateTimeOffset(session.SessionDate.ToDateTime(TimeOnly.MinValue));
            HoursPlayed = session.HoursPlayed;
            Notes = session.Notes ?? string.Empty;
        }
        else
        {
            _editingId = null;
            WindowTitle = "Přidat herní relaci";
            SessionDate = DateTimeOffset.Now;
            HoursPlayed = 1.0m;
            Notes = string.Empty;
        }

        return Task.CompletedTask;
    }

    // ── Příkazy ────────────────────────────────────────────────────

    [RelayCommand]
    private async Task Save()
    {
        ValidationError = string.Empty;

        // Validace počtu hodin
        if (HoursPlayed <= 0)
        {
            ValidationError = "Počet hodin musí být větší než 0.";
            return;
        }

        if (HoursPlayed > 24)
        {
            ValidationError = "Jedna relace nemůže trvat více než 24 hodin.";
            return;
        }

        var session = new GameSession
        {
            Id = _editingId ?? 0,
            GameId = _gameId,
            // DateTimeOffset → DateOnly
            SessionDate = DateOnly.FromDateTime(SessionDate.DateTime),
            HoursPlayed = Math.Round(HoursPlayed, 2), // Zaokrouhlíme na 2 des. místa
            Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes.Trim()
        };

        if (_editingId.HasValue)
            await _sessionRepo.UpdateAsync(session);
        else
            await _sessionRepo.AddAsync(session);

        WasSaved = true;
        RequestClose?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        RequestClose?.Invoke();
    }
}
