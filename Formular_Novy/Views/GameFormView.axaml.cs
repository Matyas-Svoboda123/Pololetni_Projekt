using Avalonia.Controls;
using Formular_Novy.ViewModels;

namespace Formular_Novy.Views;

public partial class GameFormView : Window
{
    public GameFormView()
    {
        InitializeComponent();

        // Jakmile se DataContext nastaví (z DialogService), zaregistrujeme Close na RequestClose
        // Toto je čistě UI záležitost — ViewModel neví nic o okně
        DataContextChanged += (_, _) =>
        {
            if (DataContext is GameFormViewModel vm)
                vm.RequestClose += Close;
        };
    }
}
