using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

public partial class InscriptionPage : ContentPage
{
    private double? _lat, _lon; // position GPS de l'officine (facultative)

    public InscriptionPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // clic sur "S'inscrire" : validation des champs
    private async void OnInscrire(object? sender, EventArgs e)
    {
        bool ok = true; // tout est valide tant qu'on ne trouve pas d'erreur

        if (string.IsNullOrWhiteSpace(Nom.Texte)) { Nom.MessageErreur = "Obligatoire"; ok = false; }
        string email = Email.Texte?.Trim() ?? "";
        if (!(email.Contains('@') && email.Contains('.'))) { Email.MessageErreur = "Adresse e-mail invalide"; ok = false; }
        if ((Mdp.Texte?.Length ?? 0) < 8) { Mdp.MessageErreur = "8 caractères minimum"; ok = false; }
        if (string.IsNullOrWhiteSpace(Onpc.Texte)) { Onpc.MessageErreur = "Obligatoire"; ok = false; }
        if (string.IsNullOrWhiteSpace(Officine.Texte)) { Officine.MessageErreur = "Obligatoire"; ok = false; }
        if (string.IsNullOrWhiteSpace(Adresse.Texte)) { Adresse.MessageErreur = "Obligatoire"; ok = false; }
        if (string.IsNullOrEmpty(Quartier.Valeur)) { Quartier.MessageErreur = "Obligatoire"; ok = false; }
        if ((Telephone.Texte ?? "").Count(char.IsDigit) < 8) { Telephone.MessageErreur = "Numéro invalide"; ok = false; }
        if (!CaseCertifie.IsChecked) { ErrCertifie.IsVisible = true; ok = false; }

        if (!ok) { BtnInscrire.IsEnabled = false; return; } // erreurs : bouton désactivé

        var demande = new DemandeInscription
        {
            NomComplet = Nom.Texte, Email = email, MotDePasse = Mdp.Texte, NumeroOnpc = Onpc.Texte,
            NomOfficine = Officine.Texte, Adresse = Adresse.Texte, Ville = Ville.Valeur, Quartier = Quartier.Valeur,
            Telephone = Telephone.Texte, Latitude = _lat, Longitude = _lon
        };
        string? erreur = AuthService.Inscrire(demande); // crée le compte et l'officine
        if (erreur != null) { Email.MessageErreur = erreur; BtnInscrire.IsEnabled = false; return; } // e-mail déjà pris

        // compte validé : tableau de bord ; sinon écran d'attente
        await Outils.Aller(AuthService.Courant!.Statut == StatutCompte.Valide ? "//Tableau" : "//Attente");
    }

    // toute modification réactive le bouton
    private void OnChange(object? sender, EventArgs e) => BtnInscrire.IsEnabled = true;

    // case de certification
    private void OnCertifie(object? sender, CheckedChangedEventArgs e)
    {
        ErrCertifie.IsVisible = false;
        BtnInscrire.IsEnabled = true;
    }

    // "Utiliser ma position" : GPS de l'officine
    private async void OnPosition(object? sender, EventArgs e)
    {
        var position = await LocalisationService.ObtenirPositionAsync();
        if (position == null)
        {
            LblGps.Text = "Position indisponible : le centre du quartier sera utilisé.";
            return;
        }
        _lat = position.Latitude; _lon = position.Longitude; // mémorise
        LblGps.Text = "Position de l'officine enregistrée.";
    }

    private async void OnSeConnecter(object? sender, TappedEventArgs e) => await Outils.Aller("//Connexion");
}