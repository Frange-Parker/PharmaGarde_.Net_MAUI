namespace PharmaGarde.Views;

public partial class MotDePasseOubliePage : ContentPage
{
    public MotDePasseOubliePage()
    {
        InitializeComponent(); // charge le XAML
    }

    // clic sur "Envoyer le code"
    private async void OnEnvoyer(object? sender, EventArgs e)
    {
        string email = Email.Text?.Trim() ?? ""; // e-mail saisi, sans espaces
        bool valide = email.Contains('@') && email.Contains('.'); // vérification simple du format

        if (valide)
            await Shell.Current.GoToAsync("CodeVerification"); // étape suivante : saisie du code
        else
            AfficherErreur(true); // sinon : état d'erreur
    }

    // dès qu'on retape, l'erreur disparaît
    private void OnTexteModifie(object? sender, TextChangedEventArgs e) => AfficherErreur(false);

    // active ou retire l'état d'erreur
    private void AfficherErreur(bool erreur)
    {
        ZoneErreur.IsVisible = erreur; // montre ou cache le message
        BtnEnvoyer.IsEnabled = !erreur; // bouton désactivé en cas d'erreur
        ChampEmail.Stroke = new SolidColorBrush(erreur ? Color.FromArgb("#FF8A80") : Colors.Transparent); // bordure
        ChampEmail.BackgroundColor = Color.FromArgb(erreur ? "#2A1616" : "#232323"); // fond
    }

    // retour à l'écran précédent (la connexion)
    private async void OnRetour(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");
}