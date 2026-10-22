using PharmaGarde.Models;

namespace PharmaGarde.Helpers;

// Mémoire des filtres, partagée entre la Liste et la Carte
public static class EtatFiltres
{
    public static bool Applique;                         // faux tant que l'utilisateur n'a pas appuyé sur "Appliquer"
    public static bool OuvertMaintenant = true;          // valeurs initiales = celles de la maquette
    public static bool Acces24h;
    public static HashSet<string> Quartiers = new() { "Akwa", "Bonanjo" };
    public static bool Tdr = true;
    public static bool MobileMoney = true;
    public static bool Oxygene;
    public static int RayonKm = 3;
    private static bool _navEnCours; // vrai pendant qu'une navigation est en cours

    // identifie l'état actuel (sert à savoir si la liste doit être recalculée)
    public static string Signature =>
        $"{Applique}{OuvertMaintenant}{Acces24h}{string.Join(",", Quartiers.OrderBy(q => q))}{Tdr}{MobileMoney}{Oxygene}{RayonKm}";

    // remet tous les filtres à zéro (aucune contrainte)
    public static void Reinitialiser()
    {
        Applique = false; OuvertMaintenant = false; Acces24h = false;
        Quartiers.Clear(); Tdr = false; MobileMoney = false; Oxygene = false; RayonKm = 10;
    }

    // applique les filtres et trie par distance croissante (la plus proche d'abord)
    public static List<Pharmacie> Filtrer(IEnumerable<Pharmacie> source)
    {
        var resultat = source.AsEnumerable();
        if (Applique)
        {
            resultat = resultat.Where(p => p.DistanceKm <= RayonKm);                         // dans le rayon
            if (OuvertMaintenant) resultat = resultat.Where(p => p.OuvertMaintenant);        // ouverte
            if (Acces24h) resultat = resultat.Where(p => p.Permanence24h);                   // 24h/24
            if (Quartiers.Count > 0) resultat = resultat.Where(p => Quartiers.Contains(p.Quartier)); // quartiers
            if (Tdr) resultat = resultat.Where(p => p.Tdr);                                  // services demandés
            if (MobileMoney) resultat = resultat.Where(p => p.MobileMoney);
            if (Oxygene) resultat = resultat.Where(p => p.Oxygene);
        }
        return resultat.OrderBy(p => p.DistanceKm).ToList();
    }

    // navigation protégée : ignore les touchers répétés
    public static async Task Aller(string route)
    {
        if (_navEnCours) return; // une navigation est déjà en cours
        _navEnCours = true;      // on verrouille
        try { await Shell.Current.GoToAsync(route); } // on navigue
        finally { _navEnCours = false; }              // on déverrouille
    }
}