using PharmaGarde.Helpers;

namespace PharmaGarde.Views;

[QueryProperty(nameof(Etat), "etat")] // GoToAsync("//Carte?etat=erreur")
public partial class CartePage : ContentPage
{
    public string Etat { get; set; } = ""; // "erreur" force l'état d'erreur

    public CartePage()
    {
        InitializeComponent(); // charge le XAML
    }

    protected override void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        string etat = Etat; Etat = ""; // lit puis efface l'état demandé
        bool erreur = etat == "erreur" || Outils.EstHorsLigne; // erreur forcée ou hors ligne
        Contenu.IsVisible = !erreur; // carte visible si pas d'erreur
        Erreur.IsVisible = erreur;   // erreur visible sinon
        AppliquerFiltres();          // masque les repères filtrés
    }

    // n'affiche que les repères des pharmacies qui passent les filtres
    private void AppliquerFiltres()
    {
        var ids = EtatFiltres.Filtrer(DonneesDemo.Pharmacies).Select(p => p.Id).ToHashSet();
        PinCentre.IsVisible = ids.Contains(1);
        PinCocotiers.IsVisible = ids.Contains(2);
        PinLiberte.IsVisible = ids.Contains(3);
        PinDeido.IsVisible = ids.Contains(4);
    }

    private void OnFiltres(object? sender, TappedEventArgs e) => Panneau.Afficher();     // ouvre le panneau
    private void OnFiltresAppliques(object? sender, EventArgs e) => AppliquerFiltres();   // filtres validés
    private async void OnVueListe(object? sender, TappedEventArgs e) => await Outils.Aller("//Liste");

    private async void OnAppeler(object? sender, EventArgs e) => await Outils.Appeler("+237699000001");
    private async void OnItineraire(object? sender, EventArgs e)
        => await Outils.Itineraire(4.0511, 9.7085, "Pharmacie du Centre - Akwa");
}