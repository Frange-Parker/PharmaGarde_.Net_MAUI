using Microsoft.Maui.Controls.Shapes;
using PharmaGarde.Controls;
using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

[QueryProperty(nameof(Etat), "etat")] // GoToAsync("//Planning?etat=erreur") pour montrer l'état d'erreur
public partial class PlanningPage : ContentPage
{
    public string Etat { get; set; } = ""; // état demandé par la navigation
    private bool _avenir = true;            // puce "À venir" active ?
    private bool _premier = true;           // premier affichage : montre le chargement une fois

    public PlanningPage()
    {
        InitializeComponent(); // charge le XAML
        ConstruireSquelette(); // blocs gris de chargement
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        if (!await GardePharmacien.AutoriseAsync()) return; // réservé aux pharmaciens validés
        string etat = Etat; Etat = "";
        await ChargerAsync(etat);
    }

    // charge puis affiche
    private async Task ChargerAsync(string etat)
    {
        Montrer("squelette"); // chargement
        if (etat == "erreur") { Montrer("erreur"); return; } // démonstration de l'erreur
        if (_premier) { await Task.Delay(350); _premier = false; } // chargement visible une seule fois
        Construire();
        Montrer("contenu");
    }

    // une seule variante visible
    private void Montrer(string variante)
    {
        ZoneSquelette.IsVisible = variante == "squelette";
        Contenu.IsVisible = variante == "contenu";
        Erreur.IsVisible = variante == "erreur";
    }

    // construit la liste des permanences
    private void Construire()
    {
        var officine = AuthService.Officine; // officine du pharmacien connecté
        if (officine == null) return;
        bool horsLigne = Outils.EstHorsLigne;       // hors ligne : lecture seule
        BandeauHL.IsVisible = horsLigne;
        BtnAjouter.IsEnabled = !horsLigne;

        var maintenant = DateTime.UtcNow;
        var toutes = PermanenceService.Lister(officine.Id);
        var liste = _avenir
            ? toutes.Where(x => x.FinUtc > maintenant).OrderBy(x => x.DebutUtc).ToList()        // en cours et à venir
            : toutes.Where(x => x.FinUtc <= maintenant).OrderByDescending(x => x.FinUtc).ToList(); // terminées

        ListeCartes.Children.Clear();
        foreach (var p in liste) ListeCartes.Children.Add(Carte(p, maintenant, horsLigne)); // une carte par permanence
        Vide.IsVisible = liste.Count == 0; // état vide

        // style des puces
        Colorer(ChipAvenir, TxtAvenir, _avenir);
        Colorer(ChipPassees, TxtPassees, !_avenir);
    }

    // une carte de permanence
    private View Carte(Permanence p, DateTime maintenant, bool horsLigne)
    {
        bool enCours = p.DebutUtc <= maintenant && maintenant < p.FinUtc;
        bool aVenir = p.DebutUtc > maintenant;
        string etat = enCours ? "● En cours" : aVenir ? "● À venir" : "● Terminée";
        string fond = enCours ? "#1B3A26" : aVenir ? "#14222E" : "#2A2A2A";
        string texte = enCours ? "#3DDC6F" : aVenir ? "#7CC4FF" : "#B0B0B0";

        var badge = new Border
        {
            BackgroundColor = Color.FromArgb(fond), Stroke = Colors.Transparent, Padding = new Thickness(10, 4),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
            Content = new Label { Text = etat, FontSize = 11, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb(texte) }
        };
        var entete = new Grid { ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Auto) } };
        entete.Add(new Label { Text = Libelles.Type(p.Type).ToUpperInvariant(), FontSize = 12, FontAttributes = FontAttributes.Bold,
                               CharacterSpacing = 1, TextColor = Color.FromArgb("#B0B0B0"), VerticalOptions = LayoutOptions.Center }, 0, 0);
        entete.Add(badge, 1, 0);

        var modifier = new IconButton { Text = "Modifier", Icon = Icons.Edit, Variant = "Secondary", IsEnabled = !horsLigne };
        modifier.Clicked += async (s, e) => await Outils.Aller($"Permanence?id={p.Id}"); // ouvre le formulaire
        var supprimer = new IconButton { Text = "Supprimer", Icon = Icons.Delete, Variant = "Danger", IsEnabled = !horsLigne };
        supprimer.Clicked += async (s, e) => await Supprimer(p); // supprime après confirmation
        var boutons = new Grid { ColumnSpacing = 12, ColumnDefinitions = { new ColumnDefinition(GridLength.Star), new ColumnDefinition(GridLength.Star) } };
        boutons.Add(modifier, 0, 0);
        boutons.Add(supprimer, 1, 0);

        var contenu = new VerticalStackLayout { Spacing = 10 };
        contenu.Children.Add(entete);
        contenu.Children.Add(new Label { Text = $"{Heure.JourHeure(p.DebutUtc)} - {Heure.JourHeure(p.FinUtc)}", FontSize = 17, FontAttributes = FontAttributes.Bold });
        if (!string.IsNullOrWhiteSpace(p.NoteAcces))
            contenu.Children.Add(new Label { Text = p.NoteAcces, FontSize = 13, TextColor = Color.FromArgb("#B0B0B0") });
        contenu.Children.Add(boutons);

        return new Border
        {
            BackgroundColor = Color.FromArgb("#1E1E1E"), Stroke = Color.FromArgb("#2A2A2A"),
            StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(16) }, Padding = new Thickness(16), Content = contenu
        };
    }

    // supprime une permanence après confirmation
    private async Task Supprimer(Permanence p)
    {
        bool ok = await DisplayAlert("Supprimer cette permanence ?", $"{Libelles.Type(p.Type)} du {Heure.JourHeure(p.DebutUtc)}", "Supprimer", "Annuler");
        if (!ok) return;
        PermanenceService.Supprimer(p.Id); // supprime
        Construire();                      // rafraîchit
    }

    // met une puce en style actif ou inactif
    private static void Colorer(Border puce, Label texte, bool actif)
    {
        puce.BackgroundColor = Color.FromArgb(actif ? "#00C853" : "#2A2A2A");
        texte.TextColor = Color.FromArgb(actif ? "#0B1F10" : "#EDEDED");
        texte.FontAttributes = actif ? FontAttributes.Bold : FontAttributes.None;
    }

    // deux cartes squelettes
    private void ConstruireSquelette()
    {
        for (int i = 0; i < 2; i++)
        {
            var contenu = new VerticalStackLayout { Spacing = 12 };
            foreach (var (l, h) in new[] { (110.0, 14.0), (240.0, 20.0), (200.0, 14.0), (300.0, 48.0) })
                contenu.Children.Add(new Border { BackgroundColor = Color.FromArgb("#2A2A2A"), Stroke = Colors.Transparent,
                    StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) }, WidthRequest = l, HeightRequest = h, HorizontalOptions = LayoutOptions.Start });
            Squelette.Children.Add(new Border { BackgroundColor = Color.FromArgb("#1E1E1E"), Stroke = Color.FromArgb("#2A2A2A"),
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(16) }, Padding = new Thickness(16), Content = contenu });
        }
    }

    private void OnChipAvenir(object? sender, TappedEventArgs e) { _avenir = true; Construire(); }
    private void OnChipPassees(object? sender, TappedEventArgs e) { _avenir = false; Construire(); }
    private async void OnAjouter(object? sender, EventArgs e) => await Outils.Aller("Permanence");
    private async void OnReessayer(object? sender, EventArgs e) => await ChargerAsync("");
}