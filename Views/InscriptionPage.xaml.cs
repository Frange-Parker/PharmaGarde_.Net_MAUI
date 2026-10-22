using PharmaGarde.Helpers;

namespace PharmaGarde.Views;

public partial class InscriptionPage : ContentPage
{
    public InscriptionPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // clic sur "S'inscrire" : validation des champs
    private async void OnInscrire(object? sender, EventArgs e)
    {
        bool nomOk = !string.IsNullOrWhiteSpace(Nom.Text); // le nom n'est pas vide
        bool emailOk = Email.Text != null && Email.Text.Contains('@') && Email.Text.Contains('.'); // e-mail plausible
        bool mdpOk = (Mdp.Text?.Length ?? 0) >= 8; // 8 caractères minimum

        Marquer(ChampNom, !nomOk); // bordure rouge si invalide
        Marquer(ChampEmail, !emailOk); // idem
        Marquer(ChampMdp, !mdpOk); // idem
        ErrNom.IsVisible = !nomOk; // affiche "Obligatoire"
        ErrEmail.IsVisible = !emailOk; // affiche "Invalide"
        AideMdp.TextColor = mdpOk ? Color.FromArgb("#B0B0B0") : Color.FromArgb("#FF8A80"); // aide en rouge

        if (nomOk && emailOk && mdpOk)
        {
            Session.EstInvite = false; // l'utilisateur a maintenant un compte
            await Shell.Current.GoToAsync("//Accueil"); // tout est bon : accueil
        }
        else
            BtnInscrire.IsEnabled = false; // sinon bouton désactivé
    }

    // dès qu'on retape, on réinitialise l'état d'erreur
    private void OnTexteModifie(object? sender, TextChangedEventArgs e)
    {
        Marquer(ChampNom, false); // retire le rouge
        Marquer(ChampEmail, false); // idem
        Marquer(ChampMdp, false); // idem
        ErrNom.IsVisible = false; // cache "Obligatoire"
        ErrEmail.IsVisible = false; // cache "Invalide"
        AideMdp.TextColor = Color.FromArgb("#B0B0B0"); // aide en gris
        BtnInscrire.IsEnabled = true; // réactive le bouton
    }

    // style normal ou erreur d'un champ
    private static void Marquer(Border champ, bool erreur)
    {
        champ.Stroke = new SolidColorBrush(erreur ? Color.FromArgb("#FF8A80") : Colors.Transparent); // bordure
        champ.BackgroundColor = Color.FromArgb(erreur ? "#2A1616" : "#232323"); // fond
    }

    // lien Afficher / Masquer le mot de passe
    private void OnOeil(object? sender, TappedEventArgs e)
    {
        Mdp.IsPassword = !Mdp.IsPassword; // inverse l'affichage
        TexteOeil.Text = Mdp.IsPassword ? "Afficher" : "Masquer"; // change le texte
        IconeOeil.Text = Mdp.IsPassword ? Icons.Eye : Icons.EyeOff; // change l'icône
    }

    // boîte de sélection de la ville
    private async void OnChoisirVille(object? sender, TappedEventArgs e)
    {
        string? choix = await DisplayActionSheet("Ville de référence", "Annuler", null, "Douala", "Yaoundé");
        if (choix != null && choix != "Annuler") TxtVille.Text = choix; // met à jour si choix valide
    }

    // boîte de sélection du quartier
    private async void OnChoisirQuartier(object? sender, TappedEventArgs e)
    {
        string? choix = await DisplayActionSheet("Arrondissement / Quartier", "Annuler", null,
            "Akwa / Bonanjo", "Bonapriso", "Deïdo", "Bali", "Bépanda");
        if (choix != null && choix != "Annuler") TxtQuartier.Text = choix; // met à jour si choix valide
    }

    // retour à la connexion
    private async void OnSeConnecter(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("//Connexion");
}