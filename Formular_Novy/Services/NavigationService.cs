using Formular_Novy.ViewModels;

namespace Formular_Novy.Services;

/// <summary>
/// Navigační servis — přepíná aktuální stránku v hlavním okně.
/// Udržuje referenci na MainWindowViewModel a umožňuje ViewModelům
/// navigovat mezi stránkami bez přímého přístupu na View.
/// </summary>
public class NavigationService
{
    // Nastaví se hned po vytvoření MainWindowViewModel (viz Services.cs)
    private MainWindowViewModel? _mainVm;

    public void SetMainViewModel(MainWindowViewModel vm)
    {
        _mainVm = vm;
    }

    /// <summary>
    /// Přepne CurrentPage v hlavním okně na daný ViewModel.
    /// ViewLocator z něj automaticky sestaví odpovídající View.
    /// </summary>
    public void NavigateTo(ViewModelBase vm)
    {
        if (_mainVm == null)
            throw new InvalidOperationException("NavigationService nebyl inicializován.");

        _mainVm.CurrentPage = vm;
    }
}
