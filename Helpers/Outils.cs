using System.Globalization;

namespace PharmaGarde.Helpers;

public static class Outils
{
    // vrai si le téléphone n'a pas d'accès à Internet
    public static bool EstHorsLigne => Connectivity.Current.NetworkAccess != NetworkAccess.Internet;

    private static bool _navEnCours; // vrai pendant qu'une navigation est en cours

    // navigation protégée : ignore les touchers répétés (évite d'ouvrir deux fois la même page)
    public static async Task Aller(string route)
    {
        if (_navEnCours) return; // une navigation est déjà en cours
        _navEnCours = true;      // on verrouille
        try
        {
            await Shell.Current.GoToAsync(route); // on navigue
        }
        finally
        {
            _navEnCours = false; // on déverrouille, même en cas d'erreur
        }
    }

    // ouvre l'application Téléphone avec le numéro pré-composé
    public static async Task Appeler(string numero)
    {
        try
        {
            if (PhoneDialer.Default.IsSupported) // l'appareil sait téléphoner
            {
                PhoneDialer.Default.Open(numero); // ouvre le composeur natif
                return;                           // terminé
            }
            await Launcher.Default.OpenAsync(new Uri($"tel:{numero}")); // 2e méthode : lien tel:
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Appel impossible", ex.Message, "OK"); // message clair
        }
    }

    // ouvre le GPS natif pour guider à pied jusqu'à la pharmacie
    public static async Task Itineraire(double lat, double lon, string nom)
    {
        try
        {
            var options = new MapLaunchOptions { Name = nom, NavigationMode = NavigationMode.Walking }; // à pied
            await Map.Default.OpenAsync(lat, lon, options); // lance l'application de cartes
        }
        catch (Exception)
        {
            await Shell.Current.DisplayAlert("Itinéraire", "Aucune application de cartes disponible", "OK");
        }
    }

    // mode sombre OLED : fond noir pur (#000000) ou gris très sombre (#121212)
    public static void AppliquerThemeOled(bool oled)
    {
        if (Application.Current == null) return; // application pas encore prête
        Application.Current.Resources["Fond"] = Color.FromArgb(oled ? "#000000" : "#121212"); // change la couleur de fond
    }

    // identifiants des pharmacies consultées (les plus récentes d'abord)
    public static List<int> Historique()
    {
        string texte = Preferences.Get("historique", ""); // lit "3,1,2"
        return texte.Split(',', StringSplitOptions.RemoveEmptyEntries) // découpe
            .Select(s => int.TryParse(s, out var i) ? i : 0)           // convertit
            .Where(i => i > 0)                                          // ignore les valeurs invalides
            .ToList();
    }

    // ajoute une pharmacie en tête de l'historique (20 maximum)
    public static void AjouterHistorique(int id)
    {
        var liste = Historique(); // lit l'historique
        liste.Remove(id);         // évite les doublons
        liste.Insert(0, id);      // place en tête
        Preferences.Set("historique", string.Join(",", liste.Take(20))); // enregistre
    }

    // "1,2 Mo", "340 Ko", "0 o"
    public static string Taille(long octets)
    {
        var fr = CultureInfo.GetCultureInfo("fr-FR"); // virgule décimale
        if (octets < 1024) return $"{octets} o";
        if (octets < 1024 * 1024) return (octets / 1024.0).ToString("0.#", fr) + " Ko";
        return (octets / 1048576.0).ToString("0.#", fr) + " Mo";
    }
}