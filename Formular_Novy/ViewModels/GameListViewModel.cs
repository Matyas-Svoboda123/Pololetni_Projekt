using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Formular_Novy.Models;
using Formular_Novy.Repositories;
using Formular_Novy.Services;

namespace Formular_Novy.ViewModels;

public record PlatformStat(string Name, int Count)
{
    public override string ToString() => $"{Name}: {Count}";
}

public partial class GameListViewModel : ViewModelBase
{
    private readonly IGameRepository _gameRepo;
    private readonly IPlatformRepository _platformRepo;
    private readonly IGameSessionRepository _sessionRepo;
    private readonly NavigationService _nav;
    private readonly IDialogService _dialogService;
    private readonly Func<GameDetailViewModel> _detailVmFactory;

    [ObservableProperty] private ObservableCollection<Game> _games = new();
    [ObservableProperty] private ObservableCollection<Game> _filteredGames = new();
    [ObservableProperty] private Game? _selectedGame;

    // Filtrování
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private Platform? _selectedFilterPlatform;
    [ObservableProperty] private ObservableCollection<Platform> _filterPlatforms = new();

    // Statistiky
    [ObservableProperty] private int _totalGames;
    [ObservableProperty] private decimal _totalHours;
    [ObservableProperty] private ObservableCollection<PlatformStat> _platformStats = new();

    public GameListViewModel(
        IGameRepository gameRepo,
        IPlatformRepository platformRepo,
        IGameSessionRepository sessionRepo,
        NavigationService nav,
        IDialogService dialogService,
        Func<GameDetailViewModel> detailVmFactory)
    {
        _gameRepo = gameRepo;
        _platformRepo = platformRepo;
        _sessionRepo = sessionRepo;
        _nav = nav;
        _dialogService = dialogService;
        _detailVmFactory = detailVmFactory;
    }

    public async Task LoadAsync()
    {
        var games = await _gameRepo.GetAllAsync();
        Games = new ObservableCollection<Game>(games);

        var platforms = await _platformRepo.GetAllAsync();
        FilterPlatforms = new ObservableCollection<Platform>(platforms);

        TotalHours = await _sessionRepo.GetTotalHoursAsync();

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var filtered = Games.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
            filtered = filtered.Where(g =>
                g.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                (g.Developer?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));

        if (SelectedFilterPlatform != null)
            filtered = filtered.Where(g => g.PlatformId == SelectedFilterPlatform.Id);

        FilteredGames = new ObservableCollection<Game>(filtered);

        // Pokud vybraná hra zmizela z filtrovaného seznamu, zrušíme výběr
        if (SelectedGame != null && !FilteredGames.Contains(SelectedGame))
            SelectedGame = null;

        // Statistiky vždy z celé kolekce (nezávisle na filtru)
        TotalGames = Games.Count;
        PlatformStats = new ObservableCollection<PlatformStat>(
            Games
                .GroupBy(g => g.Platform?.Name ?? "—")
                .OrderByDescending(g => g.Count())
                .Select(g => new PlatformStat(g.Key, g.Count()))
        );
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnSelectedFilterPlatformChanged(Platform? value) => ApplyFilter();

    partial void OnSelectedGameChanged(Game? value)
    {
        GoToDetailCommand.NotifyCanExecuteChanged();
        EditSelectedGameCommand.NotifyCanExecuteChanged();
        DeleteSelectedGameCommand.NotifyCanExecuteChanged();
    }

    private bool IsGameSelected => SelectedGame != null;

    [RelayCommand]
    private void ClearFilter()
    {
        SearchText = string.Empty;
        SelectedFilterPlatform = null;
    }

    [RelayCommand]
    private async Task AddGame()
    {
        var saved = await _dialogService.ShowGameFormAsync();
        if (saved) await LoadAsync();
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
        var detailVm = _detailVmFactory();
        await detailVm.LoadGameAsync(SelectedGame!.Id);
        _nav.NavigateTo(detailVm);
    }
}
