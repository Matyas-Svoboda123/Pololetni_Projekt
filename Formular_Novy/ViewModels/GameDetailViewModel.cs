using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Formular_Novy.Models;
using Formular_Novy.Repositories;
using Formular_Novy.Services;

namespace Formular_Novy.ViewModels;

/// <summary>
/// View 2 — detail hry.
/// Zobrazuje vybranou hru a seznam jejích herních relací s plným CRUD.
/// </summary>
public partial class GameDetailViewModel : ViewModelBase
{
    private readonly IGameRepository _gameRepo;
    private readonly IGameSessionRepository _sessionRepo;
    private readonly NavigationService _nav;
    private readonly IDialogService _dialogService;
    private readonly Func<GameListViewModel> _listVmFactory;

    // Aktuálně zobrazená hra
    [ObservableProperty]
    private Game? _game;

    // Seznam relací pro tuto hru
    [ObservableProperty]
    private ObservableCollection<GameSession> _sessions = new();

    // Vybraná relace v DataGridu
    [ObservableProperty]
    private GameSession? _selectedSession;

    // Celkový počet odehraných hodin — zobrazí se v headeru
    [ObservableProperty]
    private decimal _totalHours;

    public GameDetailViewModel(
        IGameRepository gameRepo,
        IGameSessionRepository sessionRepo,
        NavigationService nav,
        IDialogService dialogService,
        Func<GameListViewModel> listVmFactory)
    {
        _gameRepo = gameRepo;
        _sessionRepo = sessionRepo;
        _nav = nav;
        _dialogService = dialogService;
        _listVmFactory = listVmFactory;
    }

    /// <summary>Načte hru a její relace z DB.</summary>
    public async Task LoadGameAsync(int gameId)
    {
        Game = await _gameRepo.GetByIdAsync(gameId);
        await LoadSessionsAsync();
    }

    private async Task LoadSessionsAsync()
    {
        if (Game == null) return;

        var sessions = await _sessionRepo.GetByGameIdAsync(Game.Id);
        Sessions = new ObservableCollection<GameSession>(sessions);

        // Přepočítáme celkový čas
        TotalHours = Sessions.Sum(s => s.HoursPlayed);
    }

    // Aktualizace tlačítek při změně výběru
    partial void OnSelectedSessionChanged(GameSession? value)
    {
        EditSelectedSessionCommand.NotifyCanExecuteChanged();
        DeleteSelectedSessionCommand.NotifyCanExecuteChanged();
    }

    private bool IsSessionSelected => SelectedSession != null;

    // ── Příkazy ────────────────────────────────────────────────────

    [RelayCommand]
    private async Task GoBack()
    {
        // Vytvoříme nový GameListViewModel a přejdeme zpět
        var listVm = _listVmFactory();
        await listVm.LoadAsync();
        _nav.NavigateTo(listVm);
    }

    [RelayCommand]
    private async Task AddSession()
    {
        var saved = await _dialogService.ShowSessionFormAsync(Game!.Id);
        if (saved) await LoadSessionsAsync();
    }

    [RelayCommand(CanExecute = nameof(IsSessionSelected))]
    private async Task EditSelectedSession()
    {
        var saved = await _dialogService.ShowSessionFormAsync(Game!.Id, SelectedSession);
        if (saved) await LoadSessionsAsync();
    }

    [RelayCommand(CanExecute = nameof(IsSessionSelected))]
    private async Task DeleteSelectedSession()
    {
        var confirmed = await _dialogService.ConfirmAsync(
            $"Smazat herní relaci ze dne {SelectedSession!.SessionDate:d. M. yyyy}?");

        if (confirmed)
        {
            await _sessionRepo.DeleteAsync(SelectedSession!.Id);
            SelectedSession = null;
            await LoadSessionsAsync();
        }
    }
}
