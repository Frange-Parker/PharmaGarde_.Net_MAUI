using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

[QueryProperty(nameof(Id), "id")] // identifiant de la permanence à modifier (vide = nouvelle)
public partial class PermanencePage : ContentPage
{
    public string Id { get; set; } = "";   // identifiant reçu
    private Permanence? _existante;         // permanence en cours de modification
    private bool _initialise;               // le formulaire a-t-il déjà été rempli ?

    public PermanencePage()
    {
        InitializeComponent(); // charge le XAML
    }

    // lecture sûre des sélecteurs (valable que la propriété soit nullable ou non selon la version de MAUI)
    private static DateTime Jour(DatePicker p) => ((DateTime?)p.Date ?? DateTime.Today).Date;
    private static TimeSpan Heures(TimePicker p) => (TimeSpan?)p.Time ?? TimeSpan.Zero;

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        if (!await GardePharmacien.AutoriseAsync()) return; // réservé aux pharmaciens validés
        if (_initialise) return; // formulaire déjà rempli
        _initialise = true;

        _existante = string.IsNullOrEmpty(Id) ? null : PermanenceService.Trouver(Id); // permanence à modifier ?
        Equipe.Texte = AuthService.Courant!.NomComplet; // pharmacien de permanence par défaut

        if (_existante != null) // modification : on recopie les valeurs
        {
            LblTitre.Text = "Modifier la permanence";
            Type.Valeur = Libelles.Type(_existante.Type);
            var debut = Heure.VersLocal(_existante.DebutUtc); // heure du Cameroun
            var fin = Heure.VersLocal(_existante.FinUtc);
            DateDebut.Date = debut.Date; HeureDebut.Time = debut.TimeOfDay;
            DateFin.Date = fin.Date; HeureFin.Time = fin.TimeOfDay;
            Note.Texte = _existante.NoteAcces;
            if (!string.IsNullOrWhiteSpace(_existante.PharmacienDePermanence)) Equipe.Texte = _existante.PharmacienDePermanence;
        }
        else // nouvelle permanence : garde de nuit d'aujourd'hui 20h00 à demain 08h00
        {
            var aujourdhui = Heure.VersLocal(DateTime.UtcNow).Date;
            DateDebut.Date = aujourdhui; HeureDebut.Time = new TimeSpan(20, 0, 0);
            DateFin.Date = aujourdhui.AddDays(1); HeureFin.Time = new TimeSpan(8, 0, 0);
        }
    }

    // changement de type : propose des horaires adaptés (nouvelle permanence seulement)
    private void OnTypeChange(object? sender, EventArgs e)
    {
        EffacerErreur();
        if (_existante != null) return; // on ne touche pas aux horaires d'une permanence existante
        var jour = Jour(DateDebut);
        switch (Libelles.TypeDepuis(Type.Valeur))
        {
            case TypePermanence.Nuit:       // 20h00 -> 08h00 le lendemain
                HeureDebut.Time = new TimeSpan(20, 0, 0); DateFin.Date = jour.AddDays(1); HeureFin.Time = new TimeSpan(8, 0, 0); break;
            case TypePermanence.WeekEnd:    // 08h00 -> 08h00 deux jours plus tard
                HeureDebut.Time = new TimeSpan(8, 0, 0); DateFin.Date = jour.AddDays(2); HeureFin.Time = new TimeSpan(8, 0, 0); break;
            default:                        // jour férié ou 24h/24 : 08h00 -> 08h00 le lendemain
                HeureDebut.Time = new TimeSpan(8, 0, 0); DateFin.Date = jour.AddDays(1); HeureFin.Time = new TimeSpan(8, 0, 0); break;
        }
    }

    // toute modification efface l'erreur
    private void OnChange(object? sender, EventArgs e) => EffacerErreur();
    private void OnDateChange(object? sender, DateChangedEventArgs e) => EffacerErreur();
    private void OnHeureChange(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimePicker.Time)) EffacerErreur(); // seulement si l'heure change
    }

    private void EffacerErreur()
    {
        BlocErreur.IsVisible = false;
        BtnEnregistrer.IsEnabled = true;
    }

    // "Enregistrer"
    private async void OnEnregistrer(object? sender, EventArgs e)
    {
        if (Outils.EstHorsLigne) { MontrerErreur("Hors ligne : enregistrement impossible."); return; }

        var debutLocal = Jour(DateDebut) + Heures(HeureDebut); // date + heure saisies (heure du Cameroun)
        var finLocal = Jour(DateFin) + Heures(HeureFin);

        var p = new Permanence
        {
            Id = _existante?.Id ?? Guid.NewGuid().ToString("N"), // garde l'identifiant en modification
            PharmacieId = AuthService.Officine!.Id,
            Type = Libelles.TypeDepuis(Type.Valeur),
            DebutUtc = Heure.VersUtc(debutLocal), // stockage en UTC
            FinUtc = Heure.VersUtc(finLocal),
            NoteAcces = Note.Texte?.Trim() ?? "",
            PharmacienDePermanence = Equipe.Texte?.Trim() ?? ""
        };

        string? erreur = PermanenceService.Enregistrer(p); // contrôle les règles RG9 puis enregistre
        if (erreur != null) { MontrerErreur(erreur); return; }

        await Shell.Current.GoToAsync(".."); // retour au planning
    }

    // affiche une erreur et désactive le bouton
    private void MontrerErreur(string message)
    {
        LblErreur.Text = message;
        BlocErreur.IsVisible = true;
        BtnEnregistrer.IsEnabled = false;
    }

    private async void OnAnnuler(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
    private async void OnRetour(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync("..");
}