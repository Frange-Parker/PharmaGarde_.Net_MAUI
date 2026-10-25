namespace PharmaGarde.Helpers;

public static class Config
{
    public const bool ValidationAutomatique = true;  // vrai : un compte pharmacien est validé tout de suite (démonstration)
    public const bool ModeDemo = true;               // affiche les outils de démonstration
    public const string CodeResetDemo = "123456";    // code simulé de récupération du mot de passe
    public const int SeuilAncienJours = 7;           // au-delà : "information ancienne"
    public const string FichierServeur = "serveur_pharmagarde.json"; // base simulée (côté "serveur")
    public const string FichierCache = "cache_public.json";          // copie mémorisée pour le hors ligne

    public const string ChoixVilles = "Douala";                                      // villes proposées
    public const string ChoixQuartiers = "Akwa|Bonanjo|Bonapriso|Deïdo|Bali|Bépanda"; // quartiers, séparés par |

    // centre approximatif de chaque quartier (coordonnées de démonstration)
    private static readonly Dictionary<string, (double Lat, double Lon)> Centres = new()
    {
        ["Akwa"] = (4.0511, 9.7085),
        ["Bonanjo"] = (4.0455, 9.6925),
        ["Bonapriso"] = (4.0380, 9.7010),
        ["Deïdo"] = (4.0650, 9.7160),
        ["Bali"] = (4.0560, 9.7110),
        ["Bépanda"] = (4.0750, 9.7330)
    };

    public static Location? PositionActuelle { get; set; } // dernière position GPS obtenue (jamais envoyée à un serveur)
    public static string EmailReset { get; set; } = "";    // e-mail en cours de récupération de mot de passe

    // zone favorite choisie dans les réglages
    public static string ZoneFavorite => Preferences.Get("zone", "Akwa");

    // centre d'un quartier (Akwa si inconnu)
    public static Location CentreQuartier(string quartier)
    {
        var c = Centres.TryGetValue(quartier, out var valeur) ? valeur : Centres["Akwa"]; // cherche le quartier
        return new Location(c.Lat, c.Lon); // renvoie sa position
    }

    // vrai si on se base sur le GPS réel
    public static bool UtilisePositionGps => Preferences.Get("gps", true) && PositionActuelle != null;

    // point de référence pour calculer les distances : GPS si disponible, sinon centre de la zone favorite
    public static Location Origine() => UtilisePositionGps ? PositionActuelle! : CentreQuartier(ZoneFavorite);
}