namespace PharmaGarde.Models;

public class Pharmacie
{
    public int Id { get; set; }                          // identifiant
    public string Nom { get; set; } = "";                // nom
    public string Adresse { get; set; } = "";            // adresse complète
    public string Repere { get; set; } = "";             // repère ("Face Ancien Cinéma...")
    public string Quartier { get; set; } = "";           // quartier (filtre)
    public string Ville { get; set; } = "Douala";        // ville
    public double Latitude { get; set; }                 // position GPS
    public double Longitude { get; set; }
    public string Telephone { get; set; } = "";          // numéro à appeler
    public string HorairesOuverture { get; set; } = "08h30 - 20h00"; // horaires habituels
    public bool Tdr { get; set; }                        // service : test paludisme
    public bool MobileMoney { get; set; }                // service : Mobile Money
    public bool Oxygene { get; set; }                    // service : oxygénothérapie
    public bool Perfusion { get; set; }                  // service : perfusion et solutés
    public bool SerumAntitetanique { get; set; }         // service : sérum antitétanique, premiers secours
    public string Source { get; set; } = "Annuaire";     // "Annuaire", "Inscription" ou "Démonstration"
    public DateTime MisAJourLe { get; set; } = DateTime.UtcNow; // dernière modification (UTC)
}