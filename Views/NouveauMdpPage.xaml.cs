namespace PharmaGarde.Views;

public partial class NouveauMdpPage : ContentPage
{
    public NouveauMdpPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // clic sur "Réinitialiser le mot de passe"
    private async void OnReinitialiser(object? sender, EventArgs e)
    {
        bool longOk = (Mdp.Text?.Length ?? 0) >= 8; // 8 caractères minimum
        bool memeOk = Mdp.Text == Confirm.Text;     // les deux saisies sont identiques

        Marquer(ChampMdp, !longOk);      // bordure rouge si trop court
        Marquer(ChampConfirm, !memeOk);  // bordure rouge si différent
        ErrMdp.IsVisible = !longOk;      // message "8 caractères minimum"
        ErrConfirm.IsVisible = !memeOk;  // message "ne correspondent pas"

        if (longOk && memeOk)
        {
            // succès : message puis retour à la connexion
            await DisplayAlert("Mot de passe modifié", "Vous pouvez maintenant vous connecter.", "OK");
            await Shell.Current.GoToAsync("//Connexion");
        }
        else
            BtnReinit.IsEnabled = false; // sinon bouton désactivé
    }

    // dès qu'on retape, on réinitialise l'état d'erreur
    private void OnTexteModifie(object? sender, TextChangedEventArgs e)
    {
        Marquer(ChampMdp, false);     // retire le rouge
        Marquer(ChampConfirm, false); // idem
        ErrMdp.IsVisible = false;     // cache les messages
        ErrConfirm.IsVisible = false;
        BtnReinit.IsEnabled = true;   // réactive le bouton
    }

    // style normal ou erreur d'un champ
    private static void Marquer(Border champ, bool erreur)
    {
        champ.Stroke = new SolidColorBrush(erreur ? Color.FromArgb("#FF8A80") : Colors.Transparent); // bordure
        champ.BackgroundColor = Color.FromArgb(erreur ? "#2A1616" : "#232323"); // fond
    }

    // retour à l'écran précédent
    private async void OnRetour(object? sender, TappedEventArgs e)
        => await Shell.Current.GoToAsync("..");
}