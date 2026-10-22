namespace PharmaGarde.Helpers;

// Actions natives et outils partagés par toutes les pages
public static class Outils
{
    // vrai si le téléphone n'a pas d'accès à Internet
    public static bool EstHorsLigne =>
        Connectivity.Current.NetworkAccess != NetworkAccess.Internet;

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
            // 1re méthode : le composeur natif de MAUI
            if (PhoneDialer.Default.IsSupported)
            {
                PhoneDialer.Default.Open(numero); // ouvre le composeur avec le numéro
                return;                           // terminé
            }

            // 2e méthode : lien "tel:" ouvert par le système
            await Launcher.Default.OpenAsync(new Uri($"tel:{numero}"));
        }
        catch (Exception ex)
        {
            // aucune application Téléphone disponible : on le dit clairement
            await Shell.Current.DisplayAlert("Appel impossible", ex.Message, "OK");
        }
    }

    // ouvre le GPS natif pour guider à pied jusqu'à la pharmacie
    public static async Task Itineraire(double lat, double lon, string nom)
    {
        try
        {
            // options : nom du lieu et mode "à pied"
            var options = new MapLaunchOptions { Name = nom, NavigationMode = NavigationMode.Walking };
            await Map.Default.OpenAsync(lat, lon, options); // lance l'appli de cartes
        }
        catch (Exception) // aucune appli de cartes disponible
        {
            await Shell.Current.DisplayAlert("Itinéraire", "Aucune application de cartes disponible", "OK");
        }
    }
}