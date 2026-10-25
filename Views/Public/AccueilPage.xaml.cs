using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

[QueryProperty(nameof(Etat), "etat")] // GoToAsync("//Accueil?etat=chargement") ou "?etat=gps" (démonstration des états)
public partial class AccueilPage : ContentPage
{
    public string Etat { get; set; } = "";  // état demandé par la navigation
    private PharmacieVue? _proche;           // la pharmacie de garde la plus proche

    public AccueilPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // à chaque affichage de l'écran
    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        string etat = Etat; Etat = ""; // lit puis efface l'état demandé
        AfficherZone(); // texte "Douala-Akwa par défaut"
        if (etat == "gps") BandeauGps.IsVisible = true; // démonstration : GPS refusé
        await ChargerAsync(etat == "chargement"); // charge la pharmacie la plus proche
    }

    // texte de la zone utilisée pour les distances
    private void AfficherZone()
    {
        var texte = new FormattedString();
        texte.Spans.Add(new Span { Text = Config.UtilisePositionGps ? "Ma position" : $"Douala-{Config.ZoneFavorite}", FontAttributes = FontAttributes.Bold });
        texte.Spans.Add(new Span { Text = Config.UtilisePositionGps ? " (GPS)" : " par défaut" });
        LblZone.FormattedText = texte;
    }

    // charge les données et affiche la bonne variante
    private async Task ChargerAsync(bool figerChargement)
    {
        Squelette.IsVisible = true;      // montre le squelette
        CartePharma.IsVisible = false;   // cache la carte
        Vide.IsVisible = false;          // cache l'état vide
        if (figerChargement) return;     // démonstration : reste en chargement

        bool horsLigne = Outils.EstHorsLigne; // pas de réseau : on lit la copie mémorisée
        var resultat = await PharmacieService.ListerAsync(Config.Origine(), horsLigne); // chargement
        Squelette.IsVisible = false;     // cache le squelette

        if (resultat.CacheVide) // hors ligne et rien de mémorisé
        {
            MontrerVide(Icons.CloudOff, "Aucune donnée mémorisée", "Connectez-vous à Internet pour charger les pharmacies de garde.", "");
            return;
        }

        _proche = resultat.Pharmacies.FirstOrDefault(p => p.EstDeGarde); // la plus proche qui est de garde
        if (_proche == null) // aucune pharmacie de garde connue
        {
            MontrerVide(Icons.Search, "Aucune pharmacie de garde enregistrée", "Aucune officine de cette zone n'a déclaré de permanence en cours.", "Voir toutes les pharmacies");
            return;
        }

        CartePharma.BindingContext = _proche; // fournit les données à la carte
        CartePharma.IsVisible = true;         // l'affiche
    }

    // affiche l'état vide avec un texte
    private void MontrerVide(string icone, string titre, string message, string bouton)
    {
        Vide.Icone = icone; Vide.Titre = titre; Vide.Message = message; Vide.TexteBouton = bouton; // contenu
        Vide.IsVisible = true; // l'affiche
    }

    // "Trouver la garde autour de moi"
    private async void OnTrouverGarde(object? sender, EventArgs e)
    {
        if (!Preferences.Get("gps", true)) { BandeauGps.IsVisible = true; return; } // GPS coupé dans les réglages
        var position = await LocalisationService.ObtenirPositionAsync(); // demande l'autorisation puis lit la position
        if (position == null) { BandeauGps.IsVisible = true; return; }   // refusé ou indisponible : choix manuel
        BandeauGps.IsVisible = false;           // tout va bien
        await Outils.Aller("//Liste");          // affiche la liste triée par distance
    }

    // recherche libre : va à la liste filtrée
    private async void OnRechercher(object? sender, EventArgs e)
    {
        EtatFiltres.Texte = Recherche.Text?.Trim() ?? ""; // mémorise la recherche
        await Outils.Aller("//Liste?etat=toutes");         // liste, toutes les pharmacies
    }

    // lien "Changer" : choisit la zone par défaut
    private async void OnChangerZone(object? sender, TappedEventArgs e)
    {
        string? choix = await DisplayActionSheet("Zone par défaut", "Annuler", null, Config.ChoixQuartiers.Split('|'));
        if (string.IsNullOrEmpty(choix) || choix == "Annuler") return; // annulé
        Preferences.Set("zone", choix);   // mémorise la zone
        Config.PositionActuelle = null;   // la zone choisie remplace le GPS
        AfficherZone();                   // met à jour le texte
        await ChargerAsync(false);        // recalcule les distances
    }

    // le quartier est choisi : on active le bouton "Rechercher"
    private void OnChoixZone(object? sender, EventArgs e)
        => BtnRechercher.IsEnabled = !string.IsNullOrEmpty(ChoixQuartier.Valeur);

    // bouton "Rechercher" de l'état GPS refusé
    private async void OnRechercherZone(object? sender, EventArgs e)
    {
        Preferences.Set("zone", ChoixQuartier.Valeur); // mémorise le quartier
        Config.PositionActuelle = null;                // pas de GPS
        BandeauGps.IsVisible = false;                  // ferme le bandeau
        AfficherZone();
        await ChargerAsync(false);
    }

    // bouton "Voir toutes les pharmacies" de l'état vide
    private async void OnVoirToutes(object? sender, EventArgs e)
    {
        EtatFiltres.Texte = "";
        await Outils.Aller("//Liste?etat=toutes");
    }

    // toucher la carte : ouvre la fiche
    private async void OnOuvrirDetail(object? sender, TappedEventArgs e)
    {
        if (_proche != null) await Outils.Aller($"Detail?id={_proche.Id}&horsligne={!_proche.ItineraireActif}");
    }

    private async void OnAppeler(object? sender, EventArgs e)
    {
        if (_proche != null) await Outils.Appeler(_proche.Telephone);
    }

    private async void OnItineraire(object? sender, EventArgs e)
    {
        if (_proche != null) await Outils.Itineraire(_proche.Latitude, _proche.Longitude, _proche.Nom);
    }
}