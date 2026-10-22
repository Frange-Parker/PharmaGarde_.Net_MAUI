using PharmaGarde.Helpers;

namespace PharmaGarde.Views;

public partial class ProfilPage : ContentPage
{
    public ProfilPage()
    {
        InitializeComponent(); // charge le XAML
    }

    protected override void OnAppearing()
    {
        base.OnAppearing(); // comportement standard

        // invité : on n'affiche que le message d'invitation (et donc pas l'engrenage)
        ZoneInvite.IsVisible = Session.EstInvite;
        ZoneConnecte.IsVisible = !Session.EstInvite;
        if (Session.EstInvite) return;

        // informations modifiables (valeurs par défaut = maquette)
        LblNom.Text = Preferences.Get("nom", "Dr. JeanPaul Mbarga");
        LblEmail.Text = Preferences.Get("email", "jp.mbarga@sante.cm");
        LblTel.Text = Preferences.Get("tel", "+237 699 45 82 10");
        LblVille.Text = Preferences.Get("ville", "Douala");
        LblQuartier.Text = Preferences.Get("quartier", "Akwa");

        // officine favorite choisie depuis la fiche détail
        var favorite = DonneesDemo.Trouver(Preferences.Get("favori_id", 1));
        LblFavNom.Text = favorite.Nom;
        LblFavAdresse.Text = favorite.Adresse;
    }

    private async void OnParametres(object? sender, TappedEventArgs e) => await Outils.Aller("Parametres");
    private async void OnModifierInfos(object? sender, TappedEventArgs e) => await Outils.Aller("ModifierInfos");

    // l'historique renvoie à la liste en mode hors ligne (données mémorisées)
    private async void OnHistorique(object? sender, TappedEventArgs e) => await Outils.Aller("//Liste?etat=horsligne");

    private async void OnAppelerFavorite(object? sender, TappedEventArgs e)
        => await Outils.Appeler(DonneesDemo.Trouver(Preferences.Get("favori_id", 1)).Telephone);
    private async void OnSamu(object? sender, TappedEventArgs e) => await Outils.Appeler("119");

    // déconnexion : retour à la connexion
    private async void OnDeconnexion(object? sender, TappedEventArgs e)
    {
        Session.EstInvite = true; // plus de compte actif
        await Outils.Aller("//Connexion");
    }

    private async void OnCreerCompte(object? sender, EventArgs e) => await Outils.Aller("//Inscription");
    private async void OnSeConnecter(object? sender, TappedEventArgs e) => await Outils.Aller("//Connexion");
}