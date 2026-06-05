using Avalonia.Controls;
using Formular_Novy.ViewModels;

namespace Formular_Novy.Views;

public partial class SessionFormView : Window
{
    public SessionFormView()
    {
        InitializeComponent();

        // Zaregistrujeme zavření okna na RequestClose event z ViewModelu
        DataContextChanged += (_, _) =>
        {
            if (DataContext is SessionFormViewModel vm)
                vm.RequestClose += Close;
        };
    }
}
