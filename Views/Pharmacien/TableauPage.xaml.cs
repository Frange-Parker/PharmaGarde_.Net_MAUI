using PharmaGarde.Helpers;
using PharmaGarde.Services;
using PharmaGarde.Models;

namespace PharmaGarde.Views;

public partial class TableauPage : ContentPage
{
    public TableauPage()
    {
        InitializeComponent(); // charge le XAML
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        if (!await GardePharmacien.AutoriseAsync()) return; // réservé aux pharmaciens validés

        var pharmacien = AuthService.Courant!; // pharmacien connecté
        var officine = AuthService.Officine!;   // son officine
        LblBonjour.Text = $"Bonjour, {pharmacien.NomComplet}";
        LblOfficine.Text = officine.Nom;

        var maintenant = DateTime.UtcNow;
        var permanences = PermanenceService.Lister(officine.Id); // toutes ses permanences
        var (statut, enCours, _) = CalculStatut.Determiner(permanences, maintenant); // statut actuel (RG2)

        // carte "Statut actuel"
        if (statut == Models.StatutGarde.DeGarde && enCours != null)
        {
            LblStatut.Text = $"De garde jusqu'au {Heure.JourHeure(enCours.FinUtc)}";
            LblStatut.TextColor = Color.FromArgb("#3DDC6F");
            LblStatutDetail.Text = $"{Libelles.Type(enCours.Type)} • visible du public";
        }
        else
        {
            LblStatut.Text = "Aucune permanence en cours";
            LblStatut.TextColor = Color.FromArgb("#B0B0B0");
            LblStatutDetail.Text = "Les patients voient « Aucune donnée » ou votre prochaine garde.";
        }

        // carte "Prochaine permanence"
        var prochaine = permanences.Where(x => x.DebutUtc > maintenant).OrderBy(x => x.DebutUtc).FirstOrDefault();
        if (prochaine != null)
        {
            LblProchaine.Text = $"{Libelles.Type(prochaine.Type)} • {Heure.JourHeure(prochaine.DebutUtc)}";
            LblProchaineDetail.Text = $"Jusqu'au {Heure.JourHeure(prochaine.FinUtc)}";
        }
        else
        {
            LblProchaine.Text = "Aucune permanence à venir";
            LblProchaineDetail.Text = "Pensez à déclarer vos prochaines gardes.";
        }

        // alerte : rien en cours ni prévu dans les 7 jours
        bool couvert = permanences.Any(x => x.FinUtc > maintenant && x.DebutUtc < maintenant.AddDays(7));
        Alerte.IsVisible = !couvert;

        // dernière mise à jour de la fiche
        LblMaj.Text = $"Dernière mise à jour : {Heure.DateHeure(officine.MisAJourLe)}";

        // hors ligne : on ne peut pas déclarer
        bool horsLigne = Outils.EstHorsLigne;
        BtnDeclarer.IsEnabled = !horsLigne;
        LblHorsLigne.IsVisible = horsLigne;
    }

    private async void OnDeclarer(object? sender, EventArgs e) => await Outils.Aller("Permanence");
}