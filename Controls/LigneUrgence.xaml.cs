using PharmaGarde.Helpers;

namespace PharmaGarde.Controls;

public partial class LigneUrgence : ContentView
{
    public static readonly BindableProperty NumeroProperty = BindableProperty.Create(nameof(Numero), typeof(string), typeof(LigneUrgence), "",
        propertyChanged: (b, o, n) => ((LigneUrgence)b).LblNumero.Text = (string)n);
    public static readonly BindableProperty LibelleProperty = BindableProperty.Create(nameof(Libelle), typeof(string), typeof(LigneUrgence), "",
        propertyChanged: (b, o, n) => ((LigneUrgence)b).LblLibelle.Text = (string)n);

    public string Numero { get => (string)GetValue(NumeroProperty); set => SetValue(NumeroProperty, value); }
    public string Libelle { get => (string)GetValue(LibelleProperty); set => SetValue(LibelleProperty, value); }

    public LigneUrgence()
    {
        InitializeComponent(); // charge le XAML
    }

    // toucher : lance l'appel vers le numéro d'urgence
    private async void OnAppeler(object? sender, TappedEventArgs e) => await Outils.Appeler(Numero);
}