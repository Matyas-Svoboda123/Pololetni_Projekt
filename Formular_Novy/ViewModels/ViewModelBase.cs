using CommunityToolkit.Mvvm.ComponentModel;

namespace Formular_Novy.ViewModels;

/// <summary>
/// Základní třída pro všechny ViewModely.
/// ObservableObject z CommunityToolkit.Mvvm implementuje INotifyPropertyChanged —
/// díky tomu View automaticky reaguje na změny vlastností.
/// </summary>
public abstract class ViewModelBase : ObservableObject
{
}
