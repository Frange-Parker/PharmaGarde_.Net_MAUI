using PharmaGarde.Helpers;

namespace PharmaGarde.Views;

public partial class ConnexionPage : ContentPage
{
        // identifiants de démonstration = ceux pré-remplis dans les champs (à remplacer plus tard par l'API)
    private const string EmailDemo = "nom@exemple.fr";
    private const string MdpDemo = "motdepasse123";
    public ConnexionPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // clic sur "Se connecter"
    private async void OnConnecter(object? sender, EventArgs e)
    {
        // identifiants corrects : on entre dans l'application
        if (Email.Text == EmailDemo && Mdp.Text == MdpDemo) {
            Session.EstInvite = false; // utilisateur connecté
            await Shell.Current.GoToAsync("//Accueil");// va à l'accueil
            }
        else
            AfficherErreur(true); // sinon : état d'erreur
    }

    // dès que l'utilisateur retape quelque chose, l'erreur disparaît
    private void OnTexteModifie(object? sender, TextChangedEventArgs e) => AfficherErreur(false);

    // active ou retire l'état d'erreur
    private void AfficherErreur(bool erreur)
    {
        ZoneErreur.IsVisible = erreur; // montre ou cache le message
        BtnConnexion.IsEnabled = !erreur; // bouton désactivé en cas d'erreur
        Colorer(ChampEmail, erreur); // bordure rouge sur l'e-mail
        Colorer(ChampMdp, erreur); // bordure rouge sur le mot de passe
    }

    // applique le style normal ou erreur à un champ
    private static void Colorer(Border champ, bool erreur)
    {
        champ.Stroke = new SolidColorBrush(erreur ? Color.FromArgb("#FF8A80") : Colors.Transparent); // bordure
        champ.BackgroundColor = Color.FromArgb(erreur ? "#2A1616" : "#232323"); // fond
    }

    // affiche / masque le mot de passe
    private void OnOeil(object? sender, TappedEventArgs e)
    {
        Mdp.IsPassword = !Mdp.IsPassword; // inverse l'état
        IconeOeil.Text = Mdp.IsPassword ? Icons.EyeOff : Icons.Eye; // change l'icône
    }

        // mode invité : accès direct sans compte
    private async void OnInvite(object? sender, TappedEventArgs e)
    {
        Session.EstInvite = true;               // aucun profil disponible
        await Shell.Current.GoToAsync("//Accueil");
    }

    // lien "Créer un compte"
    private async void OnCreerCompte(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//Inscription");

        // lien "Mot de passe oublié ?" : ouvre le premier écran de récupération
    private async void OnMdpOublie(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("MotDePasseOublie");
}