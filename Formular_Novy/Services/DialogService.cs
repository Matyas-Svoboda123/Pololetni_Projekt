using Avalonia.Controls;
using Formular_Novy.Models;
using Formular_Novy.ViewModels;
using Formular_Novy.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Formular_Novy.Services;

/// <summary>
/// Implementace IDialogService — vytváří dialogová okna a předává jim ViewModely.
/// Owner (hlavní okno) se nastaví po startu aplikace z MainWindow.axaml.cs.
/// </summary>
public class DialogService : IDialogService
{
    private Window? _owner;
    private readonly IServiceProvider _serviceProvider;

    public DialogService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>Nastaví rodičovské okno pro modální dialogy.</summary>
    public void SetOwner(Window owner)
    {
        _owner = owner;
    }

    public async Task<bool> ShowGameFormAsync(Game? game = null)
    {
        // Nový ViewModel vždy přes DI (AddTransient → čistý stav)
        var vm = _serviceProvider.GetRequiredService<GameFormViewModel>();
        await vm.InitializeAsync(game);

        var window = new GameFormView { DataContext = vm };
        await window.ShowDialog(_owner!);   // Modální — čeká na zavření

        return vm.WasSaved;
    }

    public async Task<bool> ShowSessionFormAsync(int gameId, GameSession? session = null)
    {
        var vm = _serviceProvider.GetRequiredService<SessionFormViewModel>();
        await vm.InitializeAsync(gameId, session);

        var window = new SessionFormView { DataContext = vm };
        await window.ShowDialog(_owner!);

        return vm.WasSaved;
    }

    public async Task<bool> ConfirmAsync(string message)
    {
        // Vlastní potvrzovací dialog — nevyžaduje žádnou externí knihovnu
        var dialog = new ConfirmDialog(message);
        await dialog.ShowDialog(_owner!);
        return dialog.Result;
    }
}
