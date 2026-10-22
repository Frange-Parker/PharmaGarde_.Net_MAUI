using PharmaGarde.Helpers;
using PharmaGarde.Models;

namespace PharmaGarde.Views;

[QueryProperty(nameof(Etat), "etat")] // GoToAsync("//Liste?etat=erreur") ou "?etat=horsligne"
public partial class ListePage : ContentPage
{
    public string Etat { get; set; } = "";   // état demandé par la navigation
    private bool _chipOuvert;                // puce "Ouvert maintenant" active ?
    private bool _horsLigne;                 // affichage des données mémorisées ?
    private string _derniereSignature = "";  // évite de reconstruire la liste pour rien

    public ListePage()
    {
        InitializeComponent(); // charge le XAML
    }

    // identifie l'état actuel de l'écran
    private string Signature => $"{EtatFiltres.Signature}|{_chipOuvert}|{_horsLigne}";

    protected override void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        string etat = Etat; // lit l'état demandé...
        Etat = "";          // ...puis l'efface pour ne pas le garder indéfiniment

        if (etat == "horsligne") _horsLigne = true;           // demandé depuis le profil
        else if (!Outils.EstHorsLigne) _horsLigne = false;    // de retour en ligne : mode normal

        // erreur si forcée, ou si le téléphone est hors ligne et que l'utilisateur n'a pas choisi les données mémorisées
        bool erreur = etat == "erreur" || (Outils.EstHorsLigne && !_horsLigne);
        Erreur.IsVisible = erreur;                         // montre l'erreur
        Contenu.IsVisible = !erreur;                       // cache la liste
        BandeauHorsLigne.IsVisible = _horsLigne && !erreur; // bandeau rouge hors ligne

        // on ne reconstruit les cartes que si quelque chose a changé (plus rapide)
        if (!erreur && Signature != _derniereSignature) Rafraichir();
    }

    // calcule la liste à afficher et met l'écran à jour
    private void Rafraichir()
    {
        var filtrees = EtatFiltres.Filtrer(DonneesDemo.Pharmacies); // filtres du panneau
        var affichees = _chipOuvert ? filtrees.Where(p => p.OuvertMaintenant).ToList() : filtrees; // puce

        foreach (var p in DonneesDemo.Pharmacies)
        {
            p.EstPremiere = false;            // personne n'est "la plus proche"...
            p.ItineraireActif = !_horsLigne;  // GPS indisponible hors ligne
        }
        if (affichees.Count > 0) affichees[0].EstPremiere = true; // ...sauf la première de la liste

        BindableLayout.SetItemsSource(ListeCartes, affichees); // crée les cartes
        ZoneVide.IsVisible = affichees.Count == 0;             // état vide si aucun résultat

        // textes
        int n = affichees.Count;
        LblSousTitre.Text = $"Douala • {n} officine{(n > 1 ? "s" : "")} de garde ce soir";
        TxtTout.Text = $"Tout ({filtrees.Count})";

        // style des puces : la puce active est verte
        Colorer(ChipTout, TxtTout, !_chipOuvert);
        Colorer(ChipOuvert, TxtOuvert, _chipOuvert);

        _derniereSignature = Signature; // mémorise l'état affiché
    }

    // met une puce en style actif (vert) ou inactif (gris)
    private static void Colorer(Border puce, Label texte, bool actif)
    {
        puce.BackgroundColor = Color.FromArgb(actif ? "#00C853" : "#2A2A2A");
        texte.TextColor = Color.FromArgb(actif ? "#0B1F10" : "#EDEDED");
        texte.FontAttributes = actif ? FontAttributes.Bold : FontAttributes.None;
    }

    private void OnChipTout(object? sender, TappedEventArgs e) { _chipOuvert = false; Rafraichir(); }          // "Tout"
    private void OnChipOuvert(object? sender, TappedEventArgs e) { _chipOuvert = !_chipOuvert; Rafraichir(); } // bascule "Ouvert maintenant"
    private void OnFiltrer(object? sender, TappedEventArgs e) => Panneau.Afficher();                            // ouvre le panneau
    private void OnFiltresAppliques(object? sender, EventArgs e) => Rafraichir();                               // filtres validés

    // bouton "Réinitialiser les filtres" de l'état vide
    private void OnReinitialiser(object? sender, EventArgs e)
    {
        EtatFiltres.Reinitialiser(); // efface les filtres
        _chipOuvert = false;         // et la puce
        Rafraichir();
    }

    // lien "Utiliser les données hors ligne"
    private void OnDonneesHorsLigne(object? sender, TappedEventArgs e)
    {
        _horsLigne = true;                // passe en mode mémorisé
        Erreur.IsVisible = false;         // cache l'erreur
        Contenu.IsVisible = true;         // montre la liste
        BandeauHorsLigne.IsVisible = true;
        Rafraichir();
    }

    private async void OnVueCarte(object? sender, TappedEventArgs e) => await Outils.Aller("//Carte");

    // toucher sur une carte : ouvre la fiche de CETTE pharmacie
    private async void OnOuvrirDetail(object? sender, TappedEventArgs e)
    {
        if (sender is BindableObject vue && vue.BindingContext is Pharmacie p)
            await Outils.Aller($"Detail?id={p.Id}&horsligne={_horsLigne}");
    }

    // bouton Appeler d'une carte
    private async void OnAppeler(object? sender, EventArgs e)
    {
        if (sender is BindableObject vue && vue.BindingContext is Pharmacie p)
            await Outils.Appeler(p.Telephone);
    }

    // bouton Itinéraire d'une carte
    private async void OnItineraire(object? sender, EventArgs e)
    {
        if (sender is BindableObject vue && vue.BindingContext is Pharmacie p)
            await Outils.Itineraire(p.Latitude, p.Longitude, p.Nom);
    }
}