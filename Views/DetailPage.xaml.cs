using Microsoft.Maui.Controls.Shapes;
using PharmaGarde.Helpers;
using PharmaGarde.Models;

namespace PharmaGarde.Views;

[QueryProperty(nameof(Id), "id")]               // identifiant de la pharmacie à afficher
[QueryProperty(nameof(HorsLigne), "horsligne")] // "True" si on consulte les données mémorisées
public partial class DetailPage : ContentPage
{
    public string Id { get; set; } = "1";
    public string HorsLigne { get; set; } = "False";
    private Pharmacie _pharmacie = DonneesDemo.Pharmacies[0]; // pharmacie affichée

    public DetailPage()
    {
        InitializeComponent(); // charge le XAML
    }

    protected override void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        int.TryParse(Id, out int id);          // convertit l'identifiant reçu
        _pharmacie = DonneesDemo.Trouver(id);  // retrouve la pharmacie
        var p = _pharmacie;

        // textes propres à cette pharmacie
        LblNom.Text = p.Nom;
        LblRue.Text = p.Adresse.Split(',')[0];   // "Boulevard de la Liberté"
        LblRepere.Text = p.Repere;
        LblHorairesJour.Text = p.HorairesJour;
        LblLibelleGarde.Text = p.LibelleGarde;
        LblHorairesGarde.Text = p.HorairesGarde;
        PastilleTexte.Text = p.OuvertMaintenant ? "● Ouvert de garde cette nuit" : "● Fermé actuellement";
        ConstruireServices();

        // icône du favori : pleine si c'est l'officine favorite
        IcoFavori.Text = Preferences.Get("favori_id", 1) == p.Id ? Icons.Bookmark : Icons.BookmarkBorder;

        // mode hors ligne : pastille rouge, itinéraire grisé
        if (HorsLigne == "True" || Outils.EstHorsLigne)
        {
            var rouge = Color.FromArgb("#FF8A80");
            Pastille.BackgroundColor = Color.FromArgb("#3B1414");
            Pastille.Stroke = new SolidColorBrush(rouge);
            PastilleTexte.Text = "● Sauvegarde du 19/09/2026";
            PastilleTexte.TextColor = rouge;
            BtnItineraire.IsEnabled = false;
        }
    }

    // crée les 4 lignes de services
    private void ConstruireServices()
    {
        ListeServices.Children.Clear(); // repart de zéro
        var services = new (string Icone, string Titre, string Sous)[]
        {
            (Icons.Science, "TDR Paludisme & Antipaludéens", "Tests rapides et délivrance CTA d'urgence H24"),
            (Icons.Water, "Perfusion & Solutés d'urgence", "Ringer, sérum glucosé, cathéters et perfuseurs"),
            (Icons.Air, "Oxygénothérapie", "Obus d'oxygène médical et concentrateurs d'urgence"),
            (Icons.Vaccines, "Sérum antitétanique & Premiers secours", "Vaccins, antiseptiques et pansements compressifs")
        };
        foreach (var s in services)
        {
            var icone = new Label { Text = s.Icone, FontFamily = "Icons", FontSize = 18, TextColor = Color.FromArgb("#3DDC6F"),
                                    HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            var pastille = new Border { BackgroundColor = Color.FromArgb("#1E2A22"), Stroke = Colors.Transparent,
                                        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(10) },
                                        WidthRequest = 40, HeightRequest = 40, Content = icone };
            var textes = new VerticalStackLayout
            {
                VerticalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = s.Titre, FontAttributes = FontAttributes.Bold },
                    new Label { Text = s.Sous, FontSize = 12, TextColor = Color.FromArgb("#B0B0B0") }
                }
            };
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

    private async void OnRetour(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync("..");

    // partage la fiche (SMS, WhatsApp...)
    private async void OnPartager(object? sender, TappedEventArgs e)
        => await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Pharmacie de garde",
            Text = $"{_pharmacie.Nom}\n{_pharmacie.Adresse}\nTél : {_pharmacie.Telephone}"
        });

    // marque / retire l'officine favorite
    private void OnFavori(object? sender, TappedEventArgs e)
    {
        bool dejaFavori = Preferences.Get("favori_id", 1) == _pharmacie.Id;
        if (dejaFavori) { Preferences.Remove("favori_id"); IcoFavori.Text = Icons.BookmarkBorder; }
        else { Preferences.Set("favori_id", _pharmacie.Id); IcoFavori.Text = Icons.Bookmark; }
    }

    private async void OnAppeler(object? sender, EventArgs e) => await Outils.Appeler(_pharmacie.Telephone);
    private async void OnItineraire(object? sender, EventArgs e)
        => await Outils.Itineraire(_pharmacie.Latitude, _pharmacie.Longitude, _pharmacie.Nom);
}