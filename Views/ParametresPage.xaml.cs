using PharmaGarde.Helpers;

namespace PharmaGarde.Views;

public partial class ParametresPage : ContentPage
{
    public ParametresPage()
    {
        InitializeComponent(); // charge le XAML
        // relit les réglages enregistrés (par défaut : tout activé)
        SwGps.IsToggled = Preferences.Get("gps", true);
        SwOled.IsToggled = Preferences.Get("oled", true);
        SwHorsLigne.IsToggled = Preferences.Get("cache", true);
        TxtZone.Text = Preferences.Get("zone", "Douala 1er - Akwa / Bonanjo");
        MajBadge(); // état du badge Eco
    }

    // un des trois interrupteurs a changé : on enregistre et on met à jour le badge
    private void OnBasculeOption(object? sender, ToggledEventArgs e)
    {
        Preferences.Set("gps", SwGps.IsToggled);
        Preferences.Set("oled", SwOled.IsToggled);
        Preferences.Set("cache", SwHorsLigne.IsToggled);
        MajBadge();
    }

    // badge vert "Eco-actif" seulement si les deux options de sobriété sont activées
    private void MajBadge()
    {
        bool eco = SwOled.IsToggled && SwHorsLigne.IsToggled;
        TxtEco.Text = eco ? "Eco-actif" : "Eco-inactif";
        BadgeEco.BackgroundColor = Color.FromArgb(eco ? "#1B3A26" : "#2A2A2A");
        TxtEco.TextColor = Color.FromArgb(eco ? "#3DDC6F" : "#B0B0B0");
    }

    // boîte de sélection de la zone favorite
    private async void OnChoisirZone(object? sender, TappedEventArgs e)
    {
        string? choix = await DisplayActionSheet("Zone favorite par défaut", "Annuler", null,
            "Douala 1er - Akwa / Bonanjo", "Douala 2e - Deïdo", "Douala 3e - Bonapriso");
        if (choix == null || choix == "Annuler") return; // rien choisi
        TxtZone.Text = choix;                  // affiche le choix
        Preferences.Set("zone", choix);        // l'enregistre
    }

    // "Vider les fichiers en cache" : remet la taille à zéro
    private void OnViderCache(object? sender, TappedEventArgs e) => TailleCache.Text = "0 Mo libérable";

    private async void OnUrgences(object? sender, TappedEventArgs e) => await Outils.Aller("//Accueil"); // numéros d'urgence
    private async void OnLegal(object? sender, TappedEventArgs e) => await Outils.Aller("Legal");

    private async void OnDeconnexion(object? sender, TappedEventArgs e)
    {
        Session.EstInvite = true; // plus de compte actif
        await Outils.Aller("//Connexion");
    }
}