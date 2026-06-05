using CommunityToolkit.Mvvm.ComponentModel;
using Formular_Novy.Services;

namespace Formular_Novy.ViewModels;

/// <summary>
/// Hlavní ViewModel okna. Obsahuje pouze CurrentPage —
/// aktuálně zobrazenou "stránku" (jiný ViewModel).
/// ViewLocator z ní sestaví odpovídající View a zobrazí ho v ContentControl.
/// </summary>
public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase _currentPage = null!;

    // NavigationService dostane referenci na tento ViewModel, aby mohl měnit CurrentPage
    public MainWindowViewModel(GameListViewModel startPage, NavigationService nav)
    {
        _currentPage = startPage;
        nav.SetMainViewModel(this);
    }
}
