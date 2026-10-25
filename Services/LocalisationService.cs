using PharmaGarde.Helpers;

namespace PharmaGarde.Services;

public static class LocalisationService
{
    private static bool _dejaTente; // la lecture silencieuse n'est tentée qu'une fois par session

    // demande l'autorisation puis lit la position ; renvoie null si refusée ou indisponible
    public static async Task<Location?> ObtenirPositionAsync()
    {
        try
        {
            var statut = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>(); // autorisation déjà donnée ?
            if (statut != PermissionStatus.Granted)
                statut = await Permissions.RequestAsync<Permissions.LocationWhenInUse>(); // sinon on la demande
            if (statut != PermissionStatus.Granted) return null; // refusée

            var requete = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)); // précision moyenne, 10 s max
            var position = await Geolocation.Default.GetLocationAsync(requete); // lecture unique (pas de suivi continu)
            if (position != null) Config.PositionActuelle = position; // mémorisée en mémoire seulement
            return position;
        }
        catch (Exception)
        {
            return null; // GPS éteint, pas de signal, etc.
        }
    }

    // lit la dernière position connue SANS rien demander à l'utilisateur
    // (uniquement si l'autorisation a déjà été accordée) ; sinon on garde la zone favorite
    public static async Task PositionSilencieuseAsync()
    {
        if (_dejaTente || !Preferences.Get("gps", true)) return; // déjà essayé, ou GPS coupé dans les réglages
        _dejaTente = true;
        try
        {
            var statut = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (statut != PermissionStatus.Granted) return; // pas d'autorisation : on ne demande pas ici
            var position = await Geolocation.Default.GetLastKnownLocationAsync(); // rapide, sans activer le GPS
            if (position != null) Config.PositionActuelle = position;
        }
        catch (Exception) { } // en cas de problème, on garde simplement la zone favorite
    }
}