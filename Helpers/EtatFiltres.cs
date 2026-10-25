using System.Globalization;
using System.Text;
using PharmaGarde.Models;

namespace PharmaGarde.Helpers;

public static class EtatFiltres
{
    public static bool Applique;                          // faux tant que "Appliquer" n'a pas été touché
    public static bool OuvertMaintenant;                  // seulement les pharmacies de garde en ce moment
    public static bool Acces24h;                          // seulement le service continu 24h/24
    public static HashSet<string> Quartiers = new();      // quartiers cochés (vide = tous)
    public static bool Tdr;                               // service exigé : test paludisme
    public static bool MobileMoney;                       // service exigé : Mobile Money
    public static bool Oxygene;                           // service exigé : oxygénothérapie
    public static int RayonKm = 10;                       // rayon de recherche
    public static string Texte = "";                      // texte tapé dans la recherche de l'accueil

    // identifie l'état actuel (sert à savoir si l'affichage doit être recalculé)
    public static string Signature =>
        $"{Applique}{OuvertMaintenant}{Acces24h}{string.Join(",", Quartiers.OrderBy(q => q))}{Tdr}{MobileMoney}{Oxygene}{RayonKm}{Texte}";

    // remet tous les filtres à zéro
    public static void Reinitialiser()
    {
        Applique = false; OuvertMaintenant = false; Acces24h = false;
        Quartiers.Clear(); Tdr = false; MobileMoney = false; Oxygene = false; RayonKm = 10; Texte = "";
    }

    // minuscules et sans accents, pour comparer des textes
    private static string Normaliser(string s)
    {
        var sb = new StringBuilder(); // résultat
        foreach (char c in s.Normalize(NormalizationForm.FormD)) // sépare lettres et accents
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark) sb.Append(c); // garde les lettres
        return sb.ToString().ToLowerInvariant();
    }

    // applique la recherche et les filtres, puis trie par distance croissante
    public static List<PharmacieVue> Filtrer(IEnumerable<PharmacieVue> source)
    {
        var r = source; // départ : tout
        if (!string.IsNullOrWhiteSpace(Texte)) // recherche libre
        {
            string t = Normaliser(Texte.Trim());
            r = r.Where(p => Normaliser(p.Nom + " " + p.Quartier + " " + p.Pharmacie.Ville + " " + p.Adresse).Contains(t));
        }
        if (Applique) // filtres du panneau
        {
            r = r.Where(p => p.DistanceKm <= RayonKm); // dans le rayon
            if (OuvertMaintenant) r = r.Where(p => p.EstDeGarde);
            if (Acces24h) r = r.Where(p => p.EstDeGarde && p.Permanence?.Type == TypePermanence.Continu24h);
            if (Quartiers.Count > 0) r = r.Where(p => Quartiers.Contains(p.Quartier));
            if (Tdr) r = r.Where(p => p.Pharmacie.Tdr);
            if (MobileMoney) r = r.Where(p => p.Pharmacie.MobileMoney);
            if (Oxygene) r = r.Where(p => p.Pharmacie.Oxygene);
        }
        return r.OrderBy(p => p.DistanceKm).ToList(); // la plus proche d'abord
    }
}