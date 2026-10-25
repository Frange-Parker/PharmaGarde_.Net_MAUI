using Microsoft.Maui.Controls.Shapes;
using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

[QueryProperty(nameof(Id), "id")]               // identifiant de la pharmacie à afficher
[QueryProperty(nameof(HorsLigne), "horsligne")] // "True" si on consulte les données mémorisées
public partial class DetailPage : ContentPage
{
    public string Id { get; set; } = "0";
    public string HorsLigne { get; set; } = "False";
    private PharmacieVue? _vue; // pharmacie affichée

    public DetailPage()
    {
        InitializeComponent(); // charge le XAML
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        int.TryParse(Id, out int id); // identifiant reçu
        bool horsLigne = HorsLigne == "True" || Outils.EstHorsLigne; // mode hors ligne ?

        var resultat = await PharmacieService.ListerAsync(null, horsLigne); // charge les données
        _vue = resultat.Pharmacies.FirstOrDefault(p => p.Id == id);          // retrouve la pharmacie
        if (_vue == null) // introuvable
        {
            await DisplayAlert("Pharmacie introuvable", "Aucune donnée n'est disponible pour cette pharmacie.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        Outils.AjouterHistorique(_vue.Id); // mémorise la consultation
        var v = _vue;

        // textes propres à cette pharmacie
        LblNom.Text = v.Nom;
        LblRue.Text = v.Adresse.Split(',')[0];            // "Boulevard de la Liberté"
        LblRepere.Text = $"{v.Quartier}, {v.Pharmacie.Ville} • {v.Repere}";
        LblHorairesJour.Text = v.Pharmacie.HorairesOuverture;
        PastilleTexte.Text = v.BadgeTexte;
        Pastille.BackgroundColor = v.BadgeFond;
        PastilleTexte.TextColor = v.BadgeCouleur;

        // permanence : libellé et plage horaire
        LblLibelleGarde.Text = v.Statut switch
        {
            StatutGarde.DeGarde => "Permanence de garde en cours",
            StatutGarde.ProchaineGarde => "Prochaine permanence",
            StatutGarde.InformationAncienne => "Dernière permanence connue",
            _ => "Aucune permanence enregistrée"
        };
        LblHorairesGarde.Text = v.Permanence == null ? "" : v.PlageGarde;
        LblHorairesGarde.IsVisible = v.Permanence != null;

        TexteModalite.Text = v.Permanence != null && !string.IsNullOrWhiteSpace(v.Permanence.NoteAcces)
            ? v.Permanence.NoteAcces
            : "Appelez la pharmacie pour connaître les modalités d'accès.";
        LblEquipe.Text = v.Permanence != null && !string.IsNullOrWhiteSpace(v.Permanence.PharmacienDePermanence)
            ? v.Permanence.PharmacienDePermanence : "Non renseigné";
        LblFraicheur.Text = horsLigne ? $"Hors ligne : données du {Heure.DateHeure(resultat.MajUtc)}" : v.MajTexte;

        BlocAucune.IsVisible = !v.AvecDonnees; // message "Aucune donnée"
        ConstruireServices(v.Pharmacie);
        IcoFavori.Text = Preferences.Get("favori_id", 0) == v.Id ? Icons.Bookmark : Icons.BookmarkBorder; // favori ?

        // mode hors ligne : pastille rouge, itinéraire grisé
        if (horsLigne)
        {
            Pastille.BackgroundColor = Color.FromArgb("#3B1414");
            PastilleTexte.TextColor = Color.FromArgb("#FF8A80");
            PastilleTexte.Text = $"● Sauvegarde du {Heure.DateHeure(resultat.MajUtc)}";
            BtnItineraire.IsEnabled = false;
        }
    }

    // crée une ligne par service proposé
    private void ConstruireServices(Pharmacie p)
    {
        ListeServices.Children.Clear(); // repart de zéro
        var services = new List<(string Icone, string Titre, string Sous)>();
        if (p.Tdr) services.Add((Icons.Science, "TDR Paludisme & Antipaludéens", "Tests rapides et délivrance CTA d'urgence"));
        if (p.Perfusion) services.Add((Icons.Water, "Perfusion & Solutés d'urgence", "Ringer, sérum glucosé, cathéters et perfuseurs"));
        if (p.Oxygene) services.Add((Icons.Air, "Oxygénothérapie", "Obus d'oxygène médical et concentrateurs d'urgence"));
        if (p.SerumAntitetanique) services.Add((Icons.Vaccines, "Sérum antitétanique & Premiers secours", "Vaccins, antiseptiques et pansements compressifs"));
        if (p.MobileMoney) services.Add((Icons.Smartphone, "Mobile Money (OM & MoMo)", "Règlement Orange Money & MTN sans monnaie"));

        if (services.Count == 0) // aucun service renseigné
        {
            ListeServices.Children.Add(new Label { Text = "Aucun service renseigné.", FontSize = 13, TextColor = Color.FromArgb("#B0B0B0") });
            return;
        }

        foreach (var s in services)
        {
            var icone = new Label { Text = s.Icone, FontFamily = "Icons", FontSize = 18, TextColor = Color.FromArgb("#3DDC6F"),
                                    HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            var pastille = new Border { BackgroundColor = Color.FromArgb("#1E2A22"), Stroke = Colors.Transparent,
                                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
                                        WidthRequest = 40, HeightRequest = 40, Content = icone };
            var textes = new VerticalStackLayout { VerticalOptions = LayoutOptions.Center };
            textes.Children.Add(new Label { Text = s.Titre, FontAttributes = FontAttributes.Bold });
            textes.Children.Add(new Label { Text = s.Sous, FontSize = 12, TextColor = Color.FromArgb("#B0B0B0") });
            var ligne = new Grid { ColumnSpacing = 12, ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) } };
            ligne.Add(pastille, 0, 0); // icône à gauche
            ligne.Add(textes, 1, 0);   // textes à droite
            ListeServices.Children.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#161616"), Stroke = Colors.Transparent,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                Padding = new Thickness(10), Content = ligne
            });
        }
    }

    private async void OnRetour(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync(".."); // retour

    // partage la fiche (SMS, WhatsApp...)
    private async void OnPartager(object? sender, TappedEventArgs e)
    {
        if (_vue == null) return;
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Pharmacie de garde",
            Text = $"{_vue.Nom}\n{_vue.Adresse}\nTél : {_vue.Telephone}"
        });
    }

    // marque ou retire l'officine favorite
    private void OnFavori(object? sender, TappedEventArgs e)
    {
        if (_vue == null) return;
        if (Preferences.Get("favori_id", 0) == _vue.Id) { Preferences.Remove("favori_id"); IcoFavori.Text = Icons.BookmarkBorder; }
        else { Preferences.Set("favori_id", _vue.Id); IcoFavori.Text = Icons.Bookmark; }
    }

    // lien "Vous êtes le pharmacien ?"
    private async void OnInscrireOfficine(object? sender, TappedEventArgs e) => await Outils.Aller("//Inscription");

    private async void OnAppeler(object? sender, EventArgs e)
    {
        if (_vue != null) await Outils.Appeler(_vue.Telephone);
    }

    private async void OnItineraire(object? sender, EventArgs e)
    {
        if (_vue != null) await Outils.Itineraire(_vue.Latitude, _vue.Longitude, _vue.Nom);
    }
}