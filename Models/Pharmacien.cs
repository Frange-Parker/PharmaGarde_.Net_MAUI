namespace PharmaGarde.Models;

public class Pharmacien
{
    public int Id { get; set; }                       // identifiant
    public int PharmacieId { get; set; }              // son officine (0 pour un administrateur)
    public string NomComplet { get; set; } = "";      // nom du titulaire
    public string Email { get; set; } = "";           // e-mail (unique, en minuscules)
    public string MotDePasseHash { get; set; } = "";  // mot de passe haché (jamais en clair)
    public string Sel { get; set; } = "";             // sel aléatoire du hachage
    public string NumeroOnpc { get; set; } = "";      // numéro d'inscription à l'Ordre
    public StatutCompte Statut { get; set; } = StatutCompte.EnAttente; // état du compte
    public RoleCompte Role { get; set; } = RoleCompte.Pharmacien;      // pharmacien ou administrateur
    public DateTime CreeLe { get; set; } = DateTime.UtcNow;            // date de création
}