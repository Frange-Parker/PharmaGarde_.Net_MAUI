using PharmaGarde.Helpers;

namespace PharmaGarde.Views;

public partial class CodeVerificationPage : ContentPage
{
    public CodeVerificationPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // clic sur "Valider le code"
    private async void OnValider(object? sender, EventArgs e)
    {
        if (Code.Texte == Config.CodeResetDemo)
            await Outils.Aller("NouveauMdp"); // code correct : étape 3
        else
        {
            Code.MessageErreur = "Code incorrect, veuillez réessayer"; // code faux
            BtnValider.IsEnabled = false;
        }
    }

    private void OnChange(object? sender, EventArgs e) => BtnValider.IsEnabled = true;

    // renvoi du code (simulé)
    private async void OnRenvoyer(object? sender, TappedEventArgs e)
        => await DisplayAlert("Code renvoyé", $"Un nouveau code vient d'être envoyé.\n\nCode de démonstration : {Config.CodeResetDemo}", "OK");

    private async void OnRetour(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync("..");
}