using Avalonia.Controls;

namespace Formular_Novy.Views;

/// <summary>
/// Jednoduchý potvrzovací dialog (Ano/Ne).
/// Nepoužíváme externí knihovnu — výsledek vrátíme přes bool Result.
/// </summary>
public partial class ConfirmDialog : Window
{
    /// <summary>True = uživatel klikl Ano, False = Ne nebo zavřel okno.</summary>
    public bool Result { get; private set; }

    // Bezparametrový konstruktor vyžadovaný AXAML parserem
    public ConfirmDialog()
    {
        InitializeComponent();
    }

    public ConfirmDialog(string message) : this()
    {
        // Nastavíme text zprávy po inicializaci komponent
        this.FindControl<TextBlock>("MessageText")!.Text = message;

        // Tlačítko Ano — nastaví výsledek a zavře
        this.FindControl<Button>("YesButton")!.Click += (_, _) =>
        {
            Result = true;
            Close();
        };

        // Tlačítko Ne — jen zavře (Result zůstane false)
        this.FindControl<Button>("NoButton")!.Click += (_, _) =>
        {
            Result = false;
            Close();
        };
    }
}
