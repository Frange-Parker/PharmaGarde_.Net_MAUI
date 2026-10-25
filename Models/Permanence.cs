namespace PharmaGarde.Models;

public class Permanence
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N"); // identifiant unique
    public int PharmacieId { get; set; }                            // pharmacie concernée
    public DateTime DebutUtc { get; set; }                          // début (UTC)
    public DateTime FinUtc { get; set; }                            // fin (UTC)
    public TypePermanence Type { get; set; } = TypePermanence.Nuit; // type de garde
    public string NoteAcces { get; set; } = "";                     // ex. "Interphone & sonnette de nuit"
    public string PharmacienDePermanence { get; set; } = "";        // nom du pharmacien de garde
    public DateTime MisAJourLe { get; set; } = DateTime.UtcNow;     // dernière modification (UTC)
}