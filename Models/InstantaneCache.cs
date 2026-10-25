namespace PharmaGarde.Models;

// Ce que le public a le droit de voir (aucun compte pharmacien dedans)
public class InstantaneCache
{
    public DateTime MajUtc { get; set; } = DateTime.UtcNow;              // date de la copie
    public List<Pharmacie> Pharmacies { get; set; } = new();             // toutes les pharmacies
    public List<int> PharmaciesInscrites { get; set; } = new();          // celles dont le pharmacien est validé
    public List<Permanence> Permanences { get; set; } = new();           // permanences visibles (comptes validés)
}