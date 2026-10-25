using PharmaGarde.Helpers;
using PharmaGarde.Services;

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
        string email = Email.Texte?.Trim() ?? "";
        if (!(email.Contains('@') && email.Contains('.'))) { Email.MessageErreur = "Adresse e-mail invalide"; BtnEnvoyer.IsEnabled = false; return; }
        if (!AuthService.EmailExiste(email)) { Email.MessageErreur = "Aucun compte avec cet e-mail"; BtnEnvoyer.IsEnabled = false; return; }

        Config.EmailReset = email; // mémorise le compte à réinitialiser
        await DisplayAlert("Code de démonstration",
            $"Dans la version finale, ce code est envoyé par e-mail.\n\nCode : {Config.CodeResetDemo}", "OK");
        await Outils.Aller("CodeVerification"); // étape 2
    }

    private void OnChange(object? sender, EventArgs e) => BtnEnvoyer.IsEnabled = true;
    private async void OnRetour(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync("..");
}