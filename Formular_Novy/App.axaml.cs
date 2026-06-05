using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Formular_Novy.ViewModels;
using Formular_Novy.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Formular_Novy;

public partial class App : Application
{
    public override void Initialize() => AvaloniaXamlLoader.Load(this);

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // MainWindowViewModel v konstruktoru dostane GameListViewModel jako startPage.
            // Načteme data právě na TÉ instanci co je nastavena jako CurrentPage.
            var mainVm = Program.ServiceProvider.GetRequiredService<MainWindowViewModel>();

            if (mainVm.CurrentPage is GameListViewModel gameListVm)
                _ = gameListVm.LoadAsync();

            desktop.MainWindow = new MainWindow { DataContext = mainVm };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
