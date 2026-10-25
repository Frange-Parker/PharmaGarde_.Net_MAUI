using Microsoft.Maui.Controls.Shapes; // RoundRectangle
using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

// GoToAsync("//Liste?etat=...") : toutes, historique, horsligne, erreur, chargement
[QueryProperty(nameof(Etat), "etat")]
public partial class ListePage : ContentPage
{
    public string Etat { get; set; } = "";   // état demandé par la navigation
    private bool _toutes;                     // puce "Toutes" active ?
    private bool _horsLigne;                  // affiche la copie mémorisée ?
    private bool _historique;                 // affiche l'historique des pharmacies consultées ?
    private ResultatGardes? _resultat;        // dernières données chargées

    public ListePage()
    {
        InitializeComponent(); // charge le XAML
        ConstruireSquelette(); // prépare les blocs gris de chargement
    }

    // à chaque affichage de l'écran
    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        string etat = Etat; Etat = ""; // lit puis efface l'état demandé

        if (etat == "toutes") _toutes = true;                       // toutes les pharmacies
        _historique = etat == "historique";                         // historique seulement si demandé
        if (etat == "horsligne") _horsLigne = true;                 // copie mémorisée
        else if (!Outils.EstHorsLigne) _horsLigne = false;          // de retour en ligne : mode normal

        await ChargerAsync(etat);
    }

    // charge les données et choisit la variante à afficher
    private async Task ChargerAsync(string etat)
    {
        Montrer("squelette"); // montre le chargement
        if (etat == "chargement") return; // démonstration : reste en chargement
        if (etat == "erreur" || (Outils.EstHorsLigne && !_horsLigne)) { MontrerErreur(); return; } // erreur ou hors ligne

        _resultat = await PharmacieService.ListerAsync(Config.Origine(), _horsLigne); // chargement
        if (_resultat.CacheVide) { MontrerErreur(); return; } // rien de mémorisé
        Rafraichir(); // affiche la liste
    }

    // affiche l'erreur avec la date de la dernière sauvegarde si elle existe
    private void MontrerErreur()
    {
        var cache = Depot.LireCache(); // copie mémorisée
        Erreur.Message = "Vérifier votre connexion internet puis réessayer."
            + (cache != null ? $"\nDernière sauvegarde : {Heure.DateHeure(cache.MajUtc)}" : "\nAucune donnée mémorisée.");
        Erreur.TexteLien = cache != null ? "Utiliser les données hors ligne" : ""; // lien seulement s'il y a une copie
        Montrer("erreur");
    }

    // montre une seule des trois variantes
    private void Montrer(string variante)
    {
        ZoneSquelette.IsVisible = variante == "squelette";
        Contenu.IsVisible = variante == "contenu";
        Erreur.IsVisible = variante == "erreur";
    }

    // applique les filtres et construit les cartes
    private void Rafraichir()
    {
        if (_resultat == null) return; // rien à afficher

        var filtrees = EtatFiltres.Filtrer(_resultat.Pharmacies); // recherche + filtres du panneau
        if (_historique) // historique : seulement les pharmacies consultées, la plus récente d'abord
        {
            var ids = Outils.Historique();
            filtrees = filtrees.Where(p => ids.Contains(p.Id)).OrderBy(p => ids.IndexOf(p.Id)).ToList();
        }

        int nGarde = filtrees.Count(p => p.EstDeGarde); // nombre de pharmacies de garde
        var affichees = (_toutes || _historique) ? filtrees : filtrees.Where(p => p.EstDeGarde).ToList(); // RG7

        foreach (var p in filtrees) p.EstPremiere = false;         // réinitialise
        if (affichees.Count > 0) affichees[0].EstPremiere = true;  // la première porte les 2 boutons

        BindableLayout.SetItemsSource(ListeCartes, affichees);     // crée les cartes
        ZoneVide.IsVisible = affichees.Count == 0;                 // état vide s'il n'y a rien
        if (affichees.Count == 0)
        {
            Vide.Titre = _historique ? "Aucune officine consultée" : "Aucune pharmacie de garde trouvée";
            Vide.Message = _historique ? "Les pharmacies que vous ouvrez apparaîtront ici."
                                       : "Essayez d'élargir le rayon ou de changer de quartier.";
            Vide.TexteBouton = _historique ? "" : "Réinitialiser les filtres";
            Vide.IconeBouton = Icons.Tune; Vide.VarianteBouton = "Secondary";
        }

        // textes
        LblSousTitre.Text = $"Douala • {nGarde} officine{(nGarde > 1 ? "s" : "")} de garde ce soir";
        TxtGarde.Text = $"De garde ({nGarde})";
        TxtToutes.Text = $"Toutes ({filtrees.Count})";
        Colorer(ChipGarde, TxtGarde, !_toutes && !_historique); // la puce active est verte
        Colorer(ChipToutes, TxtToutes, _toutes || _historique);

        // bandeau d'information
        if (_horsLigne)
        {
            BandeauInfo.IsVisible = true;
            BandeauInfo.BackgroundColor = Color.FromArgb("#3B1414"); BandeauInfo.Stroke = new SolidColorBrush(Color.FromArgb("#FF8A80"));
            TxtBandeau.Text = $"● Mode hors ligne • Sauvegarde du {Heure.DateHeure(_resultat.MajUtc)}";
            TxtBandeau.TextColor = Color.FromArgb("#FF8A80");
        }
        else if (_historique)
        {
            BandeauInfo.IsVisible = true;
            BandeauInfo.BackgroundColor = Color.FromArgb("#2A2A2A"); BandeauInfo.Stroke = new SolidColorBrush(Colors.Transparent);
            TxtBandeau.Text = "Historique des officines consultées";
            TxtBandeau.TextColor = Color.FromArgb("#EDEDED");
        }
        else BandeauInfo.IsVisible = false;

        Montrer("contenu");
    }

    // met une puce en style actif (vert) ou inactif (gris)
    private static void Colorer(Border puce, Label texte, bool actif)
    {
        puce.BackgroundColor = Color.FromArgb(actif ? "#00C853" : "#2A2A2A");
        texte.TextColor = Color.FromArgb(actif ? "#0B1F10" : "#EDEDED");
        texte.FontAttributes = actif ? FontAttributes.Bold : FontAttributes.None;
    }

    // un bloc gris du squelette
    private static Border Os(double largeur, double hauteur) => new Border
    {
        BackgroundColor = Color.FromArgb("#2A2A2A"), Stroke = Colors.Transparent,
        StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(8) },
        HeightRequest = hauteur, WidthRequest = largeur, HorizontalOptions = LayoutOptions.Start
    };

    // construit 3 cartes squelettes
    private void ConstruireSquelette()
    {
        for (int i = 0; i < 3; i++)
        {
            var contenu = new VerticalStackLayout { Spacing = 12 };
            contenu.Children.Add(Os(110, 14));  // rubrique
            contenu.Children.Add(Os(220, 22));  // nom
            contenu.Children.Add(Os(300, 44));  // zone distance
            contenu.Children.Add(Os(260, 14));  // adresse
            contenu.Children.Add(Os(300, 48));  // boutons
            Squelette.Children.Add(new Border
            {
                BackgroundColor = Color.FromArgb("#1E1E1E"), Stroke = Color.FromArgb("#2A2A2A"),
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(16) },
                Padding = new Thickness(16), Content = contenu
            });
        }
    }

    // ----- puces et boutons -----
    private void OnChipGarde(object? sender, TappedEventArgs e) { _toutes = false; _historique = false; Rafraichir(); } // seulement les gardes
    private void OnChipToutes(object? sender, TappedEventArgs e) { _toutes = true; _historique = false; Rafraichir(); } // toutes
    private void OnFiltrer(object? sender, TappedEventArgs e) => Panneau.Afficher();   // ouvre le panneau de filtres
    private void OnFiltresAppliques(object? sender, EventArgs e) => Rafraichir();       // filtres validés

    // bouton "Réinitialiser les filtres" de l'état vide
    private void OnReinitialiser(object? sender, EventArgs e)
    {
        EtatFiltres.Reinitialiser(); // efface recherche et filtres
        _toutes = true;              // montre toutes les pharmacies
        Rafraichir();
    }

    // bouton "Réessayer"
    private async void OnReessayer(object? sender, EventArgs e)
    {
        _horsLigne = false;
        await ChargerAsync("");
    }

    // lien "Utiliser les données hors ligne"
    private async void OnDonneesHorsLigne(object? sender, EventArgs e)
    {
        _horsLigne = true;
        await ChargerAsync("");
    }

    private async void OnVueCarte(object? sender, TappedEventArgs e) => await Outils.Aller("//Carte"); // bascule vers la carte
}