using PharmaGarde.Helpers;

namespace PharmaGarde.Controls;

public partial class BarreNav : ContentView
{
    // propriété "Mode" : "Public" (4 onglets) ou "Pharmacien" (3 onglets)
    public static readonly BindableProperty ModeProperty = BindableProperty.Create(
        nameof(Mode), typeof(string), typeof(BarreNav), "Public",
        propertyChanged: (b, o, n) => ((BarreNav)b).Planifier());

    // propriété "Actif" : route de l'onglet à mettre en vert
    public static readonly BindableProperty ActifProperty = BindableProperty.Create(
        nameof(Actif), typeof(string), typeof(BarreNav), "",
        propertyChanged: (b, o, n) => ((BarreNav)b).Planifier());

    public string Mode { get => (string)GetValue(ModeProperty); set => SetValue(ModeProperty, value); }
    public string Actif { get => (string)GetValue(ActifProperty); set => SetValue(ActifProperty, value); }

    private bool _planifie; // une construction est déjà en attente ?

    public BarreNav()
    {
        InitializeComponent(); // charge le XAML
        Planifier();           // demande une construction
    }

    // regroupe les changements de Mode et d'Actif en UNE seule construction
    private void Planifier()
    {
        if (_planifie) return;   // déjà en attente
        _planifie = true;
        var d = Dispatcher;
        if (d == null) { _planifie = false; Construire(); return; } // pas de dispatcher : on construit tout de suite
        d.Dispatch(() => { _planifie = false; Construire(); });     // construit une fois que les propriétés sont posées
    }

    // dessine les onglets selon le mode
    private void Construire()
    {
        Colonnes.Children.Clear();          // retire les anciens onglets
        Colonnes.ColumnDefinitions.Clear(); // retire les anciennes colonnes

        // (route, libellé, icône) de chaque onglet
        (string Route, string Libelle, string Icone)[] onglets = Mode == "Pharmacien"
            ? new[] { ("Tableau", "Tableau", Icons.Dashboard), ("Planning", "Planning", Icons.EventNote), ("MonOfficine", "Officine", Icons.LocalPharmacy) }
            : new[] { ("Accueil", "Accueil", Icons.Home), ("Liste", "Liste", Icons.List), ("Carte", "Carte", Icons.Map), ("Reglages", "Réglages", Icons.Settings) };

        for (int i = 0; i < onglets.Length; i++)
        {
            var (route, libelle, icone) = onglets[i];
            bool actif = route == Actif; // onglet courant ?
            var couleur = Color.FromArgb(actif ? "#3DDC6F" : "#B0B0B0"); // vert ou gris

            var pile = new VerticalStackLayout { Spacing = 2 }; // icône au-dessus du libellé
            pile.Children.Add(new Label { Text = icone, FontFamily = "Icons", FontSize = 24, HorizontalOptions = LayoutOptions.Center, TextColor = couleur });
            pile.Children.Add(new Label { Text = libelle, FontSize = 11, HorizontalOptions = LayoutOptions.Center, TextColor = couleur,
                                          FontAttributes = actif ? FontAttributes.Bold : FontAttributes.None });

            var toucher = new TapGestureRecognizer();
            string cible = route; // copie locale pour la fonction ci-dessous
            toucher.Tapped += async (s, e) => await Outils.Aller("//" + cible); // va à la page correspondante
            pile.GestureRecognizers.Add(toucher);

            Colonnes.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // une colonne par onglet
            Grid.SetColumn(pile, i);
            Colonnes.Children.Add(pile);
        }
    }
}