namespace PharmaGarde.Views;

public partial class CodeVerificationPage : ContentPage
{
    // code de démonstration (à remplacer plus tard par une vérification côté serveur)
    private const string CodeDemo = "123456";

    public CodeVerificationPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // clic sur "Valider le code"
    private async void OnValider(object? sender, EventArgs e)
    {
        if (Code.Text == CodeDemo)
            await Shell.Current.GoToAsync("NouveauMdp"); // code bon : on choisit un nouveau mot de passe
        else
            AfficherErreur(true); // code faux : état d'erreur
    }

    // dès qu'on retape, l'erreur disparaît
    private void OnTexteModifie(object? sender, TextChangedEventArgs e) => AfficherErreur(false);

    // active ou retire l'état d'erreur
    private void AfficherErreur(bool erreur)
    {
        ZoneErreur.IsVisible = erreur; // montre ou cache le message
        BtnValider.IsEnabled = !erreur; // bouton désactivé en cas d'erreur
        ChampCode.Stroke = new SolidColorBrush(erreur ? Color.FromArgb("#FF8A80") : Colors.Transparent); // bordure
        ChampCode.BackgroundColor = Color.FromArgb(erreur ? "#2A1616" : "#232323"); // fond
    }

    // renvoi du code (simulé)
    private async void OnRenvoyer(object? sender, TappedEventArgs e)
        => await DisplayAlert("Code renvoyé", "Un nouveau code vient d'être envoyé.", "OK");

    // retour à l'écran précédent
    private async void OnRetour(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");
}