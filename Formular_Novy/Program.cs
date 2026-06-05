using Avalonia;
using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;

namespace Formular_Novy;

sealed class Program
{
    /// <summary>
    /// Globální DI kontejner — přístupný z celé aplikace (např. z MainWindow.axaml.cs).
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    public static void Main(string[] args)
    {
        // Načteme .env soubor (hledá ho v aktuálním adresáři i nadřazených složkách)
        Env.TraversePath().Load();

        // Sestavíme connection string z proměnných z .env
        string connectionString =
            $"Host={Env.GetString("Host")};" +
            $"Port={Env.GetString("Port")};" +
            $"Username={Env.GetString("Username")};" +
            $"Password={Env.GetString("Password")};" +
            $"Database={Env.GetString("Database")}";

        // Registrace všech závislostí (viz Services.cs)
        var services = new ServiceCollection();
        AppServices.Configure(services, connectionString);
        ServiceProvider = services.BuildServiceProvider();

        // Spustíme Avalonia aplikaci
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
