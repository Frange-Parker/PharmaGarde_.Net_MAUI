using System.Text.Json;
using PharmaGarde.Helpers;
using PharmaGarde.Models;

namespace PharmaGarde.Services;

// Stockage JSON. Pour passer à une vraie API, ce sera le seul fichier à remplacer.
public static class Depot
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = false }; // JSON compact
    private static BaseDonnees? _serveur; // copie en mémoire de la base

    private static string Chemin(string nom) => Path.Combine(FileSystem.AppDataDirectory, nom); // dossier privé de l'app

    // la base du "serveur" (chargée une seule fois)
    public static BaseDonnees Serveur
    {
        get
        {
            if (_serveur == null) _serveur = ChargerServeur(); // première utilisation : on charge
            return _serveur;
        }
    }

    // lit le fichier ; s'il n'existe pas ou est illisible, repart des données de démonstration
    private static BaseDonnees ChargerServeur()
    {
        try
        {
            string chemin = Chemin(Config.FichierServeur); // emplacement du fichier
            if (File.Exists(chemin)) // le fichier existe
            {
                var lu = JsonSerializer.Deserialize<BaseDonnees>(File.ReadAllText(chemin), Options); // lecture
                if (lu != null && lu.Pharmacies.Count > 0) return lu; // valide : on l'utilise
            }
        }
        catch (Exception) { } // fichier illisible : on l'ignore
        var neuve = DonneesDemo.Creer(); // données de démonstration
        Ecrire(neuve);                   // on les enregistre
        return neuve;
    }

    // écrit la base dans le fichier
    private static void Ecrire(BaseDonnees base_)
    {
        try
        {
            base_.MajLe = DateTime.UtcNow; // date d'écriture
            File.WriteAllText(Chemin(Config.FichierServeur), JsonSerializer.Serialize(base_, Options)); // écriture
        }
        catch (Exception) { } // disque plein ou autre : on ignore
    }

    // enregistre les modifications faites sur la base
    public static void Sauver() => Ecrire(Serveur);

    // efface tout et remet les données de démonstration
    public static void Reinitialiser()
    {
        _serveur = DonneesDemo.Creer(); // nouvelle base
        Ecrire(_serveur);               // enregistrement
        ViderCache();                   // et on supprime la copie publique
    }

    // ---------- copie publique mémorisée (mode hors ligne) ----------

    // prépare ce que le public peut voir : seulement les comptes validés
    public static InstantaneCache CreerInstantane()
    {
        var b = Serveur; // base
        var validees = b.Pharmaciens.Where(x => x.Statut == StatutCompte.Valide).Select(x => x.PharmacieId).ToHashSet(); // officines validées
        return new InstantaneCache
        {
            MajUtc = DateTime.UtcNow,                                        // date de la copie
            Pharmacies = b.Pharmacies.ToList(),                              // toutes les pharmacies
            PharmaciesInscrites = validees.ToList(),                         // officines validées
            Permanences = b.Permanences.Where(x => validees.Contains(x.PharmacieId)).ToList() // leurs permanences
        };
    }

    // lit la copie mémorisée (null si absente)
    public static InstantaneCache? LireCache()
    {
        try
        {
            string chemin = Chemin(Config.FichierCache);
            return File.Exists(chemin) ? JsonSerializer.Deserialize<InstantaneCache>(File.ReadAllText(chemin), Options) : null;
        }
        catch (Exception) { return null; }
    }

    // enregistre la copie mémorisée
    public static void EcrireCache(InstantaneCache c)
    {
        try { File.WriteAllText(Chemin(Config.FichierCache), JsonSerializer.Serialize(c, Options)); }
        catch (Exception) { }
    }

    // supprime la copie mémorisée ("Vider les fichiers en cache")
    public static void ViderCache()
    {
        try { if (File.Exists(Chemin(Config.FichierCache))) File.Delete(Chemin(Config.FichierCache)); }
        catch (Exception) { }
    }

    // taille de la copie en octets
    public static long TailleCache()
    {
        try { var f = new FileInfo(Chemin(Config.FichierCache)); return f.Exists ? f.Length : 0; }
        catch (Exception) { return 0; }
    }
}