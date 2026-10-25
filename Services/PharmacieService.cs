using PharmaGarde.Helpers;
using PharmaGarde.Models;

namespace PharmaGarde.Services;

// résultat d'un chargement
public class ResultatGardes
{
    public List<PharmacieVue> Pharmacies { get; set; } = new(); // pharmacies, triées par distance
    public DateTime MajUtc { get; set; }                        // date des données affichées
    public bool DepuisCache { get; set; }                       // lues depuis la copie mémorisée ?
    public bool CacheVide { get; set; }                         // hors ligne et aucune copie disponible
}

public static class PharmacieService
{
    private static bool _premiereCharge = true;               // simule une attente réseau une seule fois
    private static InstantaneCache? _instantane;              // dernière copie publique construite
    private static DateTime _versionBase = DateTime.MinValue; // version de la base qui a servi à la construire

    // charge les pharmacies ; horsLigne = true lit la copie mémorisée au lieu de la base
    // (le paramètre origine est conservé pour compatibilité : la position est toujours lue dans Config.Origine())
    public static async Task<ResultatGardes> ListerAsync(Location? origine, bool horsLigne)
    {
        if (_premiereCharge) { _premiereCharge = false; await Task.Delay(800); } // attente simulée au 1er chargement

        await LocalisationService.PositionSilencieuseAsync(); // lit la dernière position connue, sans rien demander

        InstantaneCache? instantane; // données à afficher
        if (horsLigne)
        {
            instantane = Depot.LireCache(); // copie mémorisée
            if (instantane == null) return new ResultatGardes { CacheVide = true, DepuisCache = true }; // rien de mémorisé
        }
        else
        {
            var version = Depot.Serveur.MajLe; // date de la dernière modification de la base
            bool change = _instantane == null || version != _versionBase; // les données ont-elles changé ?
            if (change) { _instantane = Depot.CreerInstantane(); _versionBase = version; } // on ne reconstruit que si besoin
            instantane = _instantane!;
            // on réécrit la copie sur le disque seulement si elle a changé ou si elle a été vidée
            if (Preferences.Get("cache", true) && (change || Depot.TailleCache() == 0)) Depot.EcrireCache(instantane);
        }

        var centre = Config.Origine(); // point de départ des distances : GPS ou zone favorite
        var maintenant = DateTime.UtcNow;
        var inscrites = instantane.PharmaciesInscrites.ToHashSet(); // officines à pharmacien validé
        var vues = new List<PharmacieVue>();

        foreach (var p in instantane.Pharmacies)
        {
            bool inscrite = inscrites.Contains(p.Id); // cette pharmacie a-t-elle un pharmacien validé ?
            var permanences = inscrite
                ? instantane.Permanences.Where(x => x.PharmacieId == p.Id).ToList()
                : new List<Permanence>();
            var (statut, permanence, ancienne) = CalculStatut.Determiner(permanences, maintenant); // règle RG2
            vues.Add(new PharmacieVue
            {
                Pharmacie = p,
                Statut = statut,
                Permanence = permanence,
                Ancienne = ancienne,
                EstInscrite = inscrite,
                ItineraireActif = !horsLigne, // pas d'itinéraire hors ligne
                DistanceKm = Location.CalculateDistance(centre.Latitude, centre.Longitude, p.Latitude, p.Longitude, DistanceUnits.Kilometers)
            });
        }

        return new ResultatGardes
        {
            Pharmacies = vues.OrderBy(v => v.DistanceKm).ToList(), // la plus proche d'abord
            MajUtc = instantane.MajUtc,
            DepuisCache = horsLigne
        };
    }

    // une seule pharmacie par son identifiant
    public static async Task<PharmacieVue?> ObtenirAsync(int id, bool horsLigne)
    {
        var resultat = await ListerAsync(null, horsLigne);
        return resultat.Pharmacies.FirstOrDefault(v => v.Id == id);
    }
}