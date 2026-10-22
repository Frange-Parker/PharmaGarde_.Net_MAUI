using Microsoft.Maui.Controls.Shapes; // pour RoundRectangle
using PharmaGarde.Helpers;

namespace PharmaGarde.Controls;

public partial class PanneauFiltres : ContentView
{
    // événement déclenché quand l'utilisateur appuie sur "Appliquer"
    public event EventHandler? Appliquer;

    private HashSet<string> _quartiers = new(); // copie de travail des quartiers cochés
    private int _rayon = 3;                     // copie de travail du rayon

    private static readonly string[] Secteurs = { "Akwa", "Bonanjo", "Bonapriso", "Deïdo", "Bali", "Bépanda" };
    private static readonly (int Km, string Sous)[] Rayons = { (1, "À pied"), (3, "5-8 min"), (5, "Moto-taxi"), (10, "Tout Douala") };

    public PanneauFiltres()
    {
        InitializeComponent(); // charge le XAML
    }

    // affiche le panneau avec les valeurs actuelles
    public void Afficher()
    {
        CbOuvert.IsChecked = EtatFiltres.OuvertMaintenant; // recopie l'état mémorisé
        Cb24h.IsChecked = EtatFiltres.Acces24h;
        CbTdr.IsChecked = EtatFiltres.Tdr;
        CbMm.IsChecked = EtatFiltres.MobileMoney;
        CbOxy.IsChecked = EtatFiltres.Oxygene;
        _quartiers = new HashSet<string>(EtatFiltres.Quartiers);
        _rayon = EtatFiltres.RayonKm;
        ConstruireSecteurs(); // dessine les puces
        ConstruireRayons();   // dessine les rayons
        IsVisible = true;     // montre le panneau
    }

    // dessine les puces de quartiers (vertes si cochées)
    private void ConstruireSecteurs()
    {
        ZoneSecteurs.Children.Clear(); // repart de zéro
        foreach (var quartier in Secteurs)
        {
            bool actif = _quartiers.Contains(quartier); // quartier coché ?
            var texte = new Label
            {
                Text = actif ? "✓ " + quartier : quartier,
                FontAttributes = actif ? FontAttributes.Bold : FontAttributes.None,
                TextColor = Color.FromArgb(actif ? "#0B1F10" : "#EDEDED")
            };
            var puce = new Border
            {
                BackgroundColor = Color.FromArgb(actif ? "#00C853" : "#2A2A2A"),
                Stroke = Colors.Transparent,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(16) },
                Padding = new Thickness(14, 8),
                Margin = new Thickness(0, 0, 8, 8),
                Content = texte
            };
            var tap = new TapGestureRecognizer();
            string nom = quartier; // copie locale pour le toucher
            tap.Tapped += (s, e) =>
            {
                if (!_quartiers.Add(nom)) _quartiers.Remove(nom); // coche / décoche
                ConstruireSecteurs();                              // redessine
            };
            puce.GestureRecognizers.Add(tap);
            ZoneSecteurs.Children.Add(puce);
        }
    }

    // dessine les 4 boutons de rayon (un seul sélectionné)
    private void ConstruireRayons()
    {
        ZoneRayons.Children.Clear();
        for (int i = 0; i < Rayons.Length; i++)
        {
            var (km, sous) = Rayons[i];
            bool actif = km == _rayon; // rayon sélectionné ?
            var colonne = new VerticalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Children =
                {
                    new Label { Text = $"{km} km", FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center,
                                TextColor = Color.FromArgb(actif ? "#0B1F10" : "#EDEDED") },
                    new Label { Text = sous, FontSize = 11, HorizontalOptions = LayoutOptions.Center,
                                TextColor = Color.FromArgb(actif ? "#0B1F10" : "#B0B0B0") }
                }
            };
            var bouton = new Border
            {
                BackgroundColor = Color.FromArgb(actif ? "#00C853" : "#2A2A2A"),
                Stroke = Colors.Transparent,
                StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(12) },
                Padding = new Thickness(6, 10),
                Content = colonne
            };
            var tap = new TapGestureRecognizer();
            int valeur = km; // copie locale
            tap.Tapped += (s, e) => { _rayon = valeur; ConstruireRayons(); }; // choisit ce rayon
            bouton.GestureRecognizers.Add(tap);
            Grid.SetColumn(bouton, i); // place dans la bonne colonne
            ZoneRayons.Children.Add(bouton);
        }
        ValeurRayon.Text = $"{_rayon} km"; // rappel en haut à droite
    }

    // "Appliquer les filtres" : enregistre les choix et ferme
    private void OnAppliquer(object? sender, EventArgs e)
    {
        EtatFiltres.OuvertMaintenant = CbOuvert.IsChecked; // enregistre
        EtatFiltres.Acces24h = Cb24h.IsChecked;
        EtatFiltres.Tdr = CbTdr.IsChecked;
        EtatFiltres.MobileMoney = CbMm.IsChecked;
        EtatFiltres.Oxygene = CbOxy.IsChecked;
        EtatFiltres.Quartiers = new HashSet<string>(_quartiers);
        EtatFiltres.RayonKm = _rayon;
        EtatFiltres.Applique = true; // les filtres sont désormais actifs
        IsVisible = false;           // ferme le panneau
        Appliquer?.Invoke(this, EventArgs.Empty); // prévient la page
    }

    // toucher sur la zone sombre : ferme sans appliquer
    private void OnFermer(object? sender, TappedEventArgs e) => IsVisible = false;
}