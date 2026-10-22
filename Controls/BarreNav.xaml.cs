using PharmaGarde.Helpers;

namespace PharmaGarde.Controls;

public partial class BarreNav : ContentView
{
    // propriété "Actif" utilisable en XAML : Actif="Liste" (nom de l'onglet à mettre en vert)
    public static readonly BindableProperty ActifProperty = BindableProperty.Create(
        nameof(Actif), typeof(string), typeof(BarreNav), "",
        propertyChanged: (b, o, n) => ((BarreNav)b).Actualiser()); // redessine quand la valeur change

    public string Actif
    {
        get => (string)GetValue(ActifProperty); // lit la valeur
        set => SetValue(ActifProperty, value);  // écrit la valeur
    }

    public BarreNav()
    {
        InitializeComponent(); // charge le XAML
        Actualiser();          // applique les couleurs initiales
    }

    // colore l'onglet actif en vert et les autres en gris
    private void Actualiser()
    {
        // (nom de l'onglet, son icône, son texte)
        var onglets = new[]
        {
            ("Accueil", IcoAccueil, TxtAccueil),
            ("Liste", IcoListe, TxtListe),
            ("Carte", IcoCarte, TxtCarte),
            ("Profil", IcoProfil, TxtProfil)
        };

        foreach (var (nom, icone, texte) in onglets)
        {
            bool actif = nom == Actif; // est-ce l'onglet courant ?
            var couleur = Color.FromArgb(actif ? "#3DDC6F" : "#B0B0B0"); // vert ou gris
            icone.TextColor = couleur; // couleur de l'icône
            texte.TextColor = couleur; // couleur du libellé
            texte.FontAttributes = actif ? FontAttributes.Bold : FontAttributes.None; // gras si actif
        }
    }

    // toucher sur un onglet : on va à la page correspondante
    private async void OnTape(object? sender, TappedEventArgs e)
    {
        string? cible = e.Parameter?.ToString(); // nom de la page
        if (cible != null)
            await Outils.Aller("//" + cible); // navigation protégée vers la page racine    
    }
}