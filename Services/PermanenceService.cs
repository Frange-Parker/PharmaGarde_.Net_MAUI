using PharmaGarde.Models;

namespace PharmaGarde.Services;

public static class PermanenceService
{
    // toutes les permanences d'une pharmacie, de la plus ancienne à la plus récente
    public static List<Permanence> Lister(int pharmacieId)
        => Depot.Serveur.Permanences.Where(x => x.PharmacieId == pharmacieId).OrderBy(x => x.DebutUtc).ToList();

    // une permanence par son identifiant
    public static Permanence? Trouver(string id) => Depot.Serveur.Permanences.FirstOrDefault(x => x.Id == id);

    // ajoute ou modifie ; renvoie un message d'erreur ou null si tout va bien
    public static string? Enregistrer(Permanence p)
    {
        if (p.FinUtc <= p.DebutUtc) return "La fin doit suivre le début"; // incohérence de dates

        var b = Depot.Serveur; // base
        bool chevauche = b.Permanences.Any(x => x.Id != p.Id && x.PharmacieId == p.PharmacieId
            && p.DebutUtc < x.FinUtc && x.DebutUtc < p.FinUtc); // deux périodes se recouvrent ?
        if (chevauche) return "Chevauchement avec une permanence existante";

        p.MisAJourLe = DateTime.UtcNow; // date de modification
        int index = b.Permanences.FindIndex(x => x.Id == p.Id); // existe déjà ?
        if (index >= 0) b.Permanences[index] = p; else b.Permanences.Add(p); // modifie ou ajoute

        var officine = b.Pharmacies.FirstOrDefault(x => x.Id == p.PharmacieId); // met aussi à jour la fiche
        if (officine != null) officine.MisAJourLe = DateTime.UtcNow;

        Depot.Sauver(); // écrit le fichier
        return null;
    }

    // supprime une permanence
    public static void Supprimer(string id)
    {
        Depot.Serveur.Permanences.RemoveAll(x => x.Id == id); // retire
        Depot.Sauver();                                        // écrit le fichier
    }
}