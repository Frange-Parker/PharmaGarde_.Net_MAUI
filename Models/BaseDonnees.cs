namespace PharmaGarde.Models;

public class BaseDonnees
{
    public List<Pharmacie> Pharmacies { get; set; } = new();   // annuaire des officines
    public List<Pharmacien> Pharmaciens { get; set; } = new(); // comptes pharmaciens
    public List<Permanence> Permanences { get; set; } = new(); // permanences déclarées
    public DateTime MajLe { get; set; } = DateTime.UtcNow;     // dernière écriture
}