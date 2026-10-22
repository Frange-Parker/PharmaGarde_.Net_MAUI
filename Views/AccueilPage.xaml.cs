using PharmaGarde.Helpers;

namespace PharmaGarde.Views;

[QueryProperty(nameof(Etat), "etat")] // permet GoToAsync("//Accueil?etat=chargement")
public partial class AccueilPage : ContentPage
{
    public string Etat { get; set; } = ""; // "chargement" force l'état de chargement
    private bool _dejaCharge; // évite de rejouer le chargement à chaque retour sur l'écran

    public AccueilPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // appelée à chaque affichage de l'écran
    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        if (_dejaCharge) return; // déjà chargé : on ne fait rien
        Squelette.IsVisible = true; // montre le squelette
        CartePharma.IsVisible = false; // cache la vraie carte
        if (Etat == "chargement") return; // état figé pour la démonstration
        await Task.Delay(1500); // simule l'attente de l'API
        Squelette.IsVisible = false; // cache le squelette
        CartePharma.IsVisible = true; // montre la vraie carte
        _dejaCharge = true; // mémorise
    }

    // "Trouver la garde autour de moi" : ouvre la liste des résultats
    private async void OnTrouverGarde(object? sender, EventArgs e)
        => await Shell.Current.GoToAsync("//Liste");

    // bouton Appeler : lance le composeur du téléphone
    private async void OnAppeler(object? sender, EventArgs e)
        => await Outils.Appeler("+237699000000");

    // bouton Itinéraire : lance le GPS (coordonnées d'Akwa, Douala)
    private async void OnItineraire(object? sender, EventArgs e)
        => await Outils.Itineraire(4.0511, 9.7085, "Pharmacie du Centre - Akwa");

    // clic sur un numéro d'urgence : appelle le numéro passé en paramètre
    private async void OnUrgence(object? sender, TappedEventArgs e)
        => await Outils.Appeler(e.Parameter?.ToString() ?? "119");
}