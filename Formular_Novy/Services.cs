using Formular_Novy.Repositories;
using Formular_Novy.Services;
using Formular_Novy.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Formular_Novy;

/// <summary>
/// Centrální registrace všech závislostí (Dependency Injection).
/// Program.cs zavolá AppServices.Configure() před spuštěním Avalonie.
/// Název AppServices zabraňuje konfliktu s namespace Formular_Novy.Services.
/// </summary>
public static class AppServices
{
    public static void Configure(IServiceCollection services, string connectionString)
    {
        // ── Repozitáře (Singleton — jedna instance po celou dobu běhu) ──────────
        // Singleton je vhodný, protože každý repo jen drží connection string
        // a otevírá nové připojení pro každý SQL dotaz.
        services.AddSingleton<IPlatformRepository>(new PlatformRepository(connectionString));
        services.AddSingleton<IGameRepository>(new GameRepository(connectionString));
        services.AddSingleton<IGameSessionRepository>(new GameSessionRepository(connectionString));

        // ── Navigační a dialogový servis ────────────────────────────────────────
        services.AddSingleton<NavigationService>();
        services.AddSingleton<DialogService>();
        // IDialogService ukazuje na tutéž instanci jako DialogService (potřebujeme SetOwner)
        services.AddSingleton<IDialogService>(sp => sp.GetRequiredService<DialogService>());

        // ── Továrny pro ViewModely (Func<T>) ────────────────────────────────────
        // Používáme továrny, protože GameListViewModel a GameDetailViewModel
        // jsou Transient — každá navigace dostane čistý nový instanc.
        services.AddSingleton<Func<GameListViewModel>>(
            sp => () => sp.GetRequiredService<GameListViewModel>());
        services.AddSingleton<Func<GameDetailViewModel>>(
            sp => () => sp.GetRequiredService<GameDetailViewModel>());

        // ── ViewModely (Transient — nová instance při každém resolve) ──────────
        services.AddTransient<GameListViewModel>();
        services.AddTransient<GameDetailViewModel>();
        services.AddTransient<GameFormViewModel>();
        services.AddTransient<SessionFormViewModel>();

        // MainWindowViewModel je Transient — App.axaml.cs ho dostane přes GetRequiredService
        services.AddTransient<MainWindowViewModel>();
    }
}
