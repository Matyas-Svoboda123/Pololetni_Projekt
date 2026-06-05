using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Formular_Novy.Models;
using Formular_Novy.Repositories;
using Formular_Novy.Services;

namespace Formular_Novy.ViewModels;

/// <summary>
/// View 1 — seznam her.
/// Zobrazuje všechny hry, umožňuje přidání, úpravu, smazání a přechod na detail.
/// </summary>
public partial class GameListViewModel : ViewModelBase
{
    private readonly IGameRepository _gameRepo;
    private readonly NavigationService _nav;
    private readonly IDialogService _dialogService;
    private readonly Func<GameDetailViewModel> _detailVmFactory; // Továrna pro detail VM

    [ObservableProperty]
    private ObservableCollection<Game> _games = new();

    // Vybraná hra v DataGridu — podle ní se povolují/zakazují akční tlačítka
    [ObservableProperty]
    private Game? _selectedGame;

    public GameListViewModel(
        IGameRepository gameRepo,
        NavigationService nav,
        IDialogService dialogService,
        Func<GameDetailViewModel> detailVmFactory)
    {
        _gameRepo = gameRepo;
        _nav = nav;
        _dialogService = dialogService;
        _detailVmFactory = detailVmFactory;
    }

    /// <summary>Načte všechny hry z DB a naplní kolekci Games.</summary>
    public async Task LoadAsync()
    {
        var games = await _gameRepo.GetAllAsync();
        Games = new ObservableCollection<Game>(games);
    }

    // Při změně vybrané hry informujeme příkazy, aby znovu vyhodnotily CanExecute
    partial void OnSelectedGameChanged(Game? value)
    {
        GoToDetailCommand.NotifyCanExecuteChanged();
        EditSelectedGameCommand.NotifyCanExecuteChanged();
        DeleteSelectedGameCommand.NotifyCanExecuteChanged();
    }

    private bool IsGameSelected => SelectedGame != null;

    // ── Příkazy ────────────────────────────────────────────────────

    [RelayCommand]
    private async Task AddGame()
    {
        // Otevře dialog formuláře (bez předaného game = nová hra)
        var saved = await _dialogService.ShowGameFormAsync();
        if (saved) await LoadAsync(); // Obnovíme seznam
    }

    [RelayCommand(CanExecute = nameof(IsGameSelected))]
    private async Task EditSelectedGame()
    {
        var saved = await _dialogService.ShowGameFormAsync(SelectedGame);
        if (saved) await LoadAsync();
    }

    [RelayCommand(CanExecute = nameof(IsGameSelected))]
    private async Task DeleteSelectedGame()
    {
        // Potvrzovací dialog — upozorní, že se smažou i relace (CASCADE)
        var confirmed = await _dialogService.ConfirmAsync(
            $"Opravdu smazat hru '{SelectedGame!.Title}'?\nBudou smazány i všechny herní relace.");

        if (confirmed)
        {
            await _gameRepo.DeleteAsync(SelectedGame!.Id);
            SelectedGame = null;
            await LoadAsync();
        }
    }

    [RelayCommand(CanExecute = nameof(IsGameSelected))]
    private async Task GoToDetail()
    {
        // Vytvoříme nový GameDetailViewModel přes továrnu a přejdeme na něj
        var detailVm = _detailVmFactory();
        await detailVm.LoadGameAsync(SelectedGame!.Id);
        _nav.NavigateTo(detailVm);
    }
}
