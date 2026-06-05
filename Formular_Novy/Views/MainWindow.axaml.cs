using Avalonia.Controls;
using Formular_Novy.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Formular_Novy.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    // Jakmile se okno otevře, nastavíme ho jako owner pro modální dialogy.
    // Toto je čistě UI záležitost — logika patří do DialogService.
    protected override void OnOpened(System.EventArgs e)
    {
        base.OnOpened(e);
        var dialogService = Program.ServiceProvider.GetRequiredService<DialogService>();
        dialogService.SetOwner(this);
    }
}
