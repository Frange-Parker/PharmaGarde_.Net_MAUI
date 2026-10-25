using PharmaGarde.Helpers;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

public partial class MonOfficinePage : ContentPage
{
    private double? _lat, _lon; // nouvelle position GPS (si l'utilisateur la met à jour)

    public MonOfficinePage()
    {
        InitializeComponent(); // charge le XAML
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        if (!await GardePharmacien.AutoriseAsync()) return; // réservé aux pharmaciens validés

        var o = AuthService.Officine!; // officine du pharmacien connecté
        Nom.Texte = o.Nom; Adresse.Texte = o.Adresse; Repere.Texte = o.Repere;
        Telephone.Texte = o.Telephone; Quartier.Valeur = o.Quartier; Horaires.Texte = o.HorairesOuverture;
        SwTdr.IsToggled = o.Tdr; SwMomo.IsToggled = o.MobileMoney; SwOxy.IsToggled = o.Oxygene;
        SwPerf.IsToggled = o.Perfusion; SwSerum.IsToggled = o.SerumAntitetanique;
        LblGps.Text = $"Position enregistrée : {o.Latitude:0.0000}, {o.Longitude:0.0000}";
        BtnEnregistrer.IsEnabled = !Outils.EstHorsLigne; // pas de modification hors ligne
    }

    // met à jour la position GPS
    private async void OnPosition(object? sender, EventArgs e)
    {
        var position = await LocalisationService.ObtenirPositionAsync();
        if (position == null) { LblGps.Text = "Position indisponible. Vérifiez l'autorisation et le GPS."; return; }
        _lat = position.Latitude; _lon = position.Longitude;
        LblGps.Text = $"Nouvelle position : {position.Latitude:0.0000}, {position.Longitude:0.0000} (à enregistrer)";
    }

    // "Enregistrer"
    private async void OnEnregistrer(object? sender, EventArgs e)
    {
        bool ok = true; // validation
        if (string.IsNullOrWhiteSpace(Nom.Texte)) { Nom.MessageErreur = "Obligatoire"; ok = false; }
        if (string.IsNullOrWhiteSpace(Adresse.Texte)) { Adresse.MessageErreur = "Obligatoire"; ok = false; }
        if ((Telephone.Texte ?? "").Count(char.IsDigit) < 8) { Telephone.MessageErreur = "Numéro invalide"; ok = false; }
        if (!ok) return;

        var o = AuthService.Officine!; // modifie directement la fiche
        o.Nom = Nom.Texte.Trim(); o.Adresse = Adresse.Texte.Trim(); o.Repere = Repere.Texte?.Trim() ?? "";
        o.Telephone = Telephone.Texte.Trim(); o.Quartier = Quartier.Valeur;
        o.HorairesOuverture = string.IsNullOrWhiteSpace(Horaires.Texte) ? "08h30 - 20h00" : Horaires.Texte.Trim();
        o.Tdr = SwTdr.IsToggled; o.MobileMoney = SwMomo.IsToggled; o.Oxygene = SwOxy.IsToggled;
        o.Perfusion = SwPerf.IsToggled; o.SerumAntitetanique = SwSerum.IsToggled;
        if (_lat != null && _lon != null) { o.Latitude = _lat.Value; o.Longitude = _lon.Value; _lat = null; _lon = null; }
        o.MisAJourLe = DateTime.UtcNow; // date de modification
        Depot.Sauver();                 // écrit le fichier

        await DisplayAlert("Enregistré", "La fiche de votre officine a été mise à jour.", "OK");
    }

    // déconnexion
    private async void OnDeconnexion(object? sender, TappedEventArgs e)
    {
        AuthService.Deconnecter();
        await Outils.Aller("//Accueil");
    }
}