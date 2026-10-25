using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

public partial class ReglagesPage : ContentPage
{
    private bool _chargement; // vrai pendant qu'on remplit l'écran (évite d'enregistrer par erreur)

    public ReglagesPage()
    {
        InitializeComponent(); // charge le XAML
    }

    protected override void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        _chargement = true; // on remplit l'écran
        SwGps.IsToggled = Preferences.Get("gps", true);     // relit les réglages enregistrés
        SwOled.IsToggled = Preferences.Get("oled", true);
        SwCache.IsToggled = Preferences.Get("cache", true);
        ChoixZone.Valeur = Config.ZoneFavorite;
        _chargement = false;
        MajBadge(); MajCache(); MajFavori();
        BlocDemo.IsVisible = Config.ModeDemo; // outil de démonstration
    }

    // un interrupteur a changé : enregistre et met à jour
    private void OnBasculeOption(object? sender, ToggledEventArgs e)
    {
        if (_chargement) return; // changement dû au remplissage de l'écran
        Preferences.Set("gps", SwGps.IsToggled);
        Preferences.Set("oled", SwOled.IsToggled);
        Preferences.Set("cache", SwCache.IsToggled);
        Outils.AppliquerThemeOled(SwOled.IsToggled); // fond noir pur ou non, tout de suite
        if (!SwCache.IsToggled) Depot.ViderCache();  // mémorisation coupée : on supprime la copie
        MajBadge(); MajCache();
    }

    // badge vert "Eco-actif" seulement si les deux options de sobriété sont actives
    private void MajBadge()
    {
        bool eco = SwOled.IsToggled && SwCache.IsToggled;
        TxtEco.Text = eco ? "Eco-actif" : "Eco-inactif";
        BadgeEco.BackgroundColor = Color.FromArgb(eco ? "#1B3A26" : "#2A2A2A");
        TxtEco.TextColor = Color.FromArgb(eco ? "#3DDC6F" : "#B0B0B0");
    }

    // taille réelle de la copie mémorisée
    private void MajCache()
    {
        long taille = Depot.TailleCache();
        TailleCache.Text = taille == 0 ? "Cache vide" : $"{Outils.Taille(taille)} libérables";
    }

    // nom de l'officine favorite
    private void MajFavori()
    {
        int id = Preferences.Get("favori_id", 0);
        var p = Depot.Serveur.Pharmacies.FirstOrDefault(x => x.Id == id);
        LblFavori.Text = p?.Nom ?? "Aucune officine favorite";
    }

    // zone favorite choisie
    private void OnChoixZone(object? sender, EventArgs e) => Preferences.Set("zone", ChoixZone.Valeur);

    // "Vider les fichiers en cache"
    private void OnViderCache(object? sender, TappedEventArgs e)
    {
        Depot.ViderCache(); // supprime la copie
        MajCache();         // affiche "Cache vide"
    }

    // ouvre la fiche de l'officine favorite
    private async void OnOuvrirFavori(object? sender, TappedEventArgs e)
    {
        int id = Preferences.Get("favori_id", 0);
        if (id > 0) await Outils.Aller($"Detail?id={id}&horsligne=False");
    }

    // historique : la liste des pharmacies déjà consultées
    private async void OnHistorique(object? sender, TappedEventArgs e) => await Outils.Aller("//Liste?etat=historique");

    // numéros d'urgence : sur l'accueil
    private async void OnUrgences(object? sender, TappedEventArgs e) => await Outils.Aller("//Accueil");

    private async void OnLegal(object? sender, TappedEventArgs e) => await Outils.Aller("Legal");

    // espace pharmacien : selon l'état du compte
    private async void OnEspacePharmacien(object? sender, TappedEventArgs e)
    {
        var p = AuthService.Courant; // pharmacien déjà connecté ?
        if (p == null) await Outils.Aller("//Connexion");
        else if (p.Statut == StatutCompte.Valide) await Outils.Aller("//Tableau");
        else await Outils.Aller("//Attente");
    }

    // remet les données de démonstration
    private async void OnReinitialiserDemo(object? sender, TappedEventArgs e)
    {
        bool ok = await DisplayAlert("Réinitialiser", "Toutes les données saisies seront remplacées par les données de démonstration.", "Réinitialiser", "Annuler");
        if (!ok) return;
        Depot.Reinitialiser();           // base et cache
        AuthService.Deconnecter();       // plus de pharmacien connecté
        Preferences.Remove("favori_id"); // favori et historique
        Preferences.Remove("historique");
        EtatFiltres.Reinitialiser();     // filtres
        MajCache(); MajFavori();
        await DisplayAlert("Terminé", "Les données de démonstration ont été remises à zéro.", "OK");
    }
}