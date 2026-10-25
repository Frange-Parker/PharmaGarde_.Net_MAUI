using PharmaGarde.Models;

namespace PharmaGarde.Services;

public static class AdminService
{
    public const string EmailAdmin = "admin@pharmagarde.cm"; // identifiant de démonstration
    private const string MdpAdmin = "Admin2026";             // mot de passe de démonstration (jamais en dur en production)

    // vrai si la personne connectée est administrateur
    public static bool EstAdminConnecte => AuthService.Courant?.Role == RoleCompte.Admin;

    // crée le compte administrateur s'il n'existe pas (aussi après une réinitialisation des données)
    public static void AssurerCompteAdmin()
    {
        var b = Depot.Serveur; // base
        if (b.Pharmaciens.Any(x => x.Role == RoleCompte.Admin)) return; // il existe déjà

        string sel = Securite.NouveauSel(); // sel du mot de passe
        b.Pharmaciens.Add(new Pharmacien
        {
            Id = b.Pharmaciens.Count == 0 ? 1 : b.Pharmaciens.Max(x => x.Id) + 1,
            PharmacieId = 0, // un administrateur n'a pas d'officine
            NomComplet = "Administrateur PharmaGarde",
            Email = EmailAdmin,
            Sel = sel,
            MotDePasseHash = Securite.Hacher(MdpAdmin, sel),
            NumeroOnpc = "-",
            Statut = StatutCompte.Valide,
            Role = RoleCompte.Admin
        });
        Depot.Sauver(); // enregistre
    }

    // tous les comptes pharmaciens, les plus récents d'abord
    public static List<Pharmacien> Comptes()
        => Depot.Serveur.Pharmaciens.Where(p => p.Role == RoleCompte.Pharmacien).OrderByDescending(p => p.CreeLe).ToList();

    // l'officine d'un compte
    public static Pharmacie? OfficineDe(Pharmacien p)
        => Depot.Serveur.Pharmacies.FirstOrDefault(x => x.Id == p.PharmacieId);

    // change le statut d'un compte (valider, refuser, suspendre, réactiver)
    public static void ChangerStatut(int id, StatutCompte statut)
    {
        var p = Depot.Serveur.Pharmaciens.FirstOrDefault(x => x.Id == id && x.Role == RoleCompte.Pharmacien);
        if (p == null) return; // compte introuvable
        p.Statut = statut;     // nouveau statut
        Depot.Sauver();        // enregistre : la liste publique se met à jour
    }

    // supprime un compte, ses permanences, et son officine si elle a été créée par inscription
    public static void Supprimer(int id)
    {
        var b = Depot.Serveur;
        var p = b.Pharmaciens.FirstOrDefault(x => x.Id == id && x.Role == RoleCompte.Pharmacien);
        if (p == null) return;
        b.Pharmaciens.Remove(p); // retire le compte

        bool autreCompte = b.Pharmaciens.Any(x => x.PharmacieId == p.PharmacieId); // l'officine a-t-elle un autre compte ?
        if (!autreCompte)
        {
            b.Permanences.RemoveAll(x => x.PharmacieId == p.PharmacieId); // retire ses permanences
            var officine = b.Pharmacies.FirstOrDefault(x => x.Id == p.PharmacieId);
            if (officine != null && officine.Source == "Inscription") b.Pharmacies.Remove(officine); // supprime les fiches créées par inscription
        }
        Depot.Sauver();
    }
}