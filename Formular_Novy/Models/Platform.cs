namespace Formular_Novy.Models;

/// <summary>
/// Číselník platforem — PC, PlayStation, Xbox, Nintendo Switch, Mobile.
/// Tato tabulka se neplní z UI, jen se načítá (seed.sql ji naplní při startu DB).
/// </summary>
public class Platform
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Přepíšeme ToString, aby se v ComboBoxu zobrazil název platformy
    public override string ToString() => Name;
}
