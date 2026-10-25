using System.Globalization;
using System.Text.Json;
using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

[QueryProperty(nameof(Etat), "etat")] // GoToAsync("//Carte?etat=erreur") pour montrer l'état d'erreur
public partial class CartePage : ContentPage
{
    // Clé gratuite CARTO (https://carto.com/basemaps/apikey/). Collez-la entre les guillemets.
    // Laissée vide : le fond de carte Esri (sans clé) est utilisé à la place.
    private const string CleCarto = "";

    // Page HTML de la carte : Leaflet + fond de carte sombre.
    // __DONNEES__ = pharmacies (JSON) ; __LAT__ / __LON__ = position de départ ;
    // __TUILES__ / __ZOOM_MAX__ / __ATTRIBUTION__ = fond de carte choisi par le code C#.
    private const string ModeleHtml = @"<!DOCTYPE html>
<html><head><meta charset='utf-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
<link rel='stylesheet' href='https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/leaflet.min.css'>
<style>
html,body,#carte{height:100%;margin:0;background:#121212;}
.leaflet-container{background:#121212;font-family:sans-serif;}
.etiquette{background:#0d0d0d;color:#ededed;border:none;font-size:11px;box-shadow:none;}
.leaflet-tooltip-top:before{border-top-color:#0d0d0d;}
.leaflet-tooltip-bottom:before{border-bottom-color:#0d0d0d;}
</style></head>
<body><div id='carte'></div>
<script src='https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/leaflet.min.js'></script>
<script>
if (typeof L === 'undefined') {
  document.body.innerHTML = '<p style=""color:#b0b0b0;font-family:sans-serif;padding:24px"">Carte indisponible : vérifiez votre connexion.</p>';
} else {
  var donnees = __DONNEES__;
  var carte = L.map('carte', {zoomControl: false});
  L.tileLayer('__TUILES__', {maxZoom: __ZOOM_MAX__, subdomains: 'abcd', attribution: '__ATTRIBUTION__'}).addTo(carte);
  var points = [[__LAT__, __LON__]];
  L.circleMarker([__LAT__, __LON__], {radius: 8, color: '#4C8DFF', fillColor: '#4C8DFF', fillOpacity: 0.9})
    .addTo(carte).bindTooltip('Vous êtes ici', {permanent: true, direction: 'bottom', className: 'etiquette'});
  donnees.forEach(function (p) {
    var couleur = p.garde ? '#00C853' : '#6B6B6B';
    var m = L.circleMarker([p.lat, p.lon], {radius: p.garde ? 12 : 9, color: couleur, fillColor: couleur, fillOpacity: 0.95}).addTo(carte);
    m.bindTooltip(p.nom + ' • ' + p.distance, {permanent: true, direction: 'top', className: 'etiquette'});
    m.on('click', function () { window.location.href = 'pharma://select?id=' + p.id; });
    points.push([p.lat, p.lon]);
  });
  carte.fitBounds(points, {padding: [40, 40], maxZoom: 16});
}
</script></body></html>";

    public string Etat { get; set; } = "";        // état demandé par la navigation
    private List<PharmacieVue> _vues = new();     // pharmacies affichées
    private PharmacieVue? _selection;             // pharmacie sélectionnée (panneau du bas)
    private string _signature = "";               // empreinte de la dernière carte chargée

    public CartePage()
    {
        InitializeComponent(); // charge le XAML
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        string etat = Etat; Etat = ""; // lit puis efface l'état demandé
        await ChargerAsync(etat);
    }

    // charge les données et affiche la carte (ou l'erreur)
    private async Task ChargerAsync(string etat)
    {
        // la carte a besoin d'Internet : sans réseau, état d'erreur
        if (etat == "erreur" || Outils.EstHorsLigne) { Montrer(false); return; }

        var resultat = await PharmacieService.ListerAsync(null, false); // données actuelles
        _vues = EtatFiltres.Filtrer(resultat.Pharmacies);               // mêmes filtres que la liste

        int idPrecedent = _selection?.Id ?? 0; // garde la sélection si possible
        _selection = _vues.FirstOrDefault(p => p.Id == idPrecedent)
                     ?? _vues.FirstOrDefault(p => p.EstDeGarde)
                     ?? _vues.FirstOrDefault();

        Montrer(true); // montre la carte
        MajPanneau();  // met à jour le panneau du bas

        // la carte n'est rechargée que si les repères ont changé (évite d'attendre à chaque visite)
        var origine = Config.Origine();
        string signature = string.Join(",", _vues.Select(v => v.Id + (v.EstDeGarde ? "g" : "n")))
                           + "|" + origine.Latitude.ToString("0.0000", CultureInfo.InvariantCulture)
                           + "|" + origine.Longitude.ToString("0.0000", CultureInfo.InvariantCulture);
        if (signature != _signature)
        {
            _signature = signature;
            ChargerCarte(origine);
        }
    }

    // construit la page HTML et la charge dans le WebView
    private void ChargerCarte(Location origine)
    {
        var donnees = _vues.Select(v => new
        {
            id = v.Id, nom = v.Nom, lat = v.Latitude, lon = v.Longitude, garde = v.EstDeGarde, distance = v.DistanceTexte
        });

        // choix du fond de carte : CARTO si une clé est fournie, sinon Esri (sans clé)
        bool avecCle = !string.IsNullOrWhiteSpace(CleCarto);
        string tuiles = avecCle
            ? $"https://{{s}}.basemaps.cartocdn.com/rastertiles/dark_all/{{z}}/{{x}}/{{y}}.png?key={CleCarto.Trim()}"
            : "https://server.arcgisonline.com/ArcGIS/rest/services/Canvas/World_Dark_Gray_Base/MapServer/tile/{z}/{y}/{x}";
        string attribution = avecCle ? "&copy; OpenStreetMap contributors &copy; CARTO" : "Tiles &copy; Esri";
        string zoomMax = avecCle ? "19" : "16"; // le fond Esri s'arrête au zoom 16

        string html = ModeleHtml
            .Replace("__DONNEES__", JsonSerializer.Serialize(donnees))
            .Replace("__TUILES__", tuiles)
            .Replace("__ZOOM_MAX__", zoomMax)
            .Replace("__ATTRIBUTION__", attribution)
            .Replace("__LAT__", origine.Latitude.ToString(CultureInfo.InvariantCulture))
            .Replace("__LON__", origine.Longitude.ToString(CultureInfo.InvariantCulture));
        Web.Source = new HtmlWebViewSource { Html = html, BaseUrl = "https://localhost/" }; // charge la carte
    }

    // carte normale ou erreur
    private void Montrer(bool carte)
    {
        Web.IsVisible = carte;
        Erreur.IsVisible = !carte;
        if (!carte) { PanneauBas.IsVisible = false; LblAucune.IsVisible = false; }
    }

    // panneau du bas : la pharmacie sélectionnée
    private void MajPanneau()
    {
        PanneauBas.IsVisible = _selection != null;
        LblAucune.IsVisible = _selection == null;
        PanneauBas.BindingContext = _selection;
    }

    // la page web envoie "pharma://select?id=3" quand on touche un repère
    private void OnNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (!e.Url.StartsWith("pharma://")) return; // navigation normale : on ne touche à rien
        e.Cancel = true;                            // message de la carte : on bloque la navigation
        int pos = e.Url.IndexOf("id=", StringComparison.Ordinal);
        if (pos < 0 || !int.TryParse(e.Url[(pos + 3)..], out int id)) return;
        var choisie = _vues.FirstOrDefault(v => v.Id == id); // pharmacie touchée
        if (choisie == null) return;
        MainThread.BeginInvokeOnMainThread(() => { _selection = choisie; MajPanneau(); }); // met à jour le panneau
    }

    // le chargement de la page a échoué : état d'erreur
    private void OnNavigated(object? sender, WebNavigatedEventArgs e)
    {
        if (e.Result == WebNavigationResult.Failure || e.Result == WebNavigationResult.Timeout) Montrer(false);
    }

    private async void OnReessayer(object? sender, EventArgs e) { _signature = ""; await ChargerAsync(""); } // bouton "Réessayer"
    private async void OnListeHorsLigne(object? sender, EventArgs e) => await Outils.Aller("//Liste?etat=horsligne");
    private async void OnVueListe(object? sender, TappedEventArgs e) => await Outils.Aller("//Liste");
    private void OnFiltres(object? sender, TappedEventArgs e) => Panneau.Afficher();
    private async void OnFiltresAppliques(object? sender, EventArgs e) => await ChargerAsync("");

    private async void OnAppeler(object? sender, EventArgs e)
    {
        if (_selection != null) await Outils.Appeler(_selection.Telephone);
    }

    private async void OnItineraire(object? sender, EventArgs e)
    {
        if (_selection != null) await Outils.Itineraire(_selection.Latitude, _selection.Longitude, _selection.Nom);
    }
}