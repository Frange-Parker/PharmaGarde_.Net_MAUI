using PharmaGarde.Helpers;
using PharmaGarde.Services;

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
        bool longOk = (Mdp.Texte?.Length ?? 0) >= 8; // 8 caractères minimum
        bool memeOk = Mdp.Texte == Confirm.Texte;     // les deux saisies sont identiques

        if (!longOk) Mdp.MessageErreur = "8 caractères minimum";
        if (!memeOk) Confirm.MessageErreur = "Les mots de passe ne correspondent pas";
        if (!longOk || !memeOk) { BtnReinit.IsEnabled = false; return; }

        if (string.IsNullOrEmpty(Config.EmailReset)) { await Outils.Aller("//Connexion"); return; } // pas de compte en cours

        AuthService.ChangerMotDePasse(Config.EmailReset, Mdp.Texte); // enregistre le nouveau mot de passe
        Config.EmailReset = "";                                        // oublie le compte
        await DisplayAlert("Mot de passe modifié", "Vous pouvez maintenant vous connecter.", "OK");
        await Outils.Aller("//Connexion"); // retour à la connexion
    }

    private void OnChange(object? sender, EventArgs e) => BtnReinit.IsEnabled = true;
    private async void OnRetour(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync("..");
}