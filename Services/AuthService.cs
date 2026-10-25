using PharmaGarde.Helpers;
using PharmaGarde.Models;

namespace PharmaGarde.Services;

// formulaire d'inscription
public class DemandeInscription
{
    public string NomComplet { get; set; } = "";
    public string Email { get; set; } = "";
    public string MotDePasse { get; set; } = "";
    public string NumeroOnpc { get; set; } = "";
    public string NomOfficine { get; set; } = "";
    public string Adresse { get; set; } = "";
    public string Ville { get; set; } = "Douala";
    public string Quartier { get; set; } = "Akwa";
    public string Telephone { get; set; } = "";
    public double? Latitude { get; set; }   // position GPS de l'officine (facultative)
    public double? Longitude { get; set; }
}

public static class AuthService
{
    public static Pharmacien? Courant { get; private set; } // pharmacien connecté (null si personne)
    private static int _echecs;                              // nombre d'échecs de connexion consécutifs
    private static DateTime _bloqueJusqua = DateTime.MinValue; // blocage temporaire après trop d'échecs

    // retrouve la session de l'ouverture précédente
    public static void RestaurerSession()
    {
        int id = Preferences.Get("pharmacien_id", 0); // identifiant mémorisé
        Courant = id == 0 ? null : Depot.Serveur.Pharmaciens.FirstOrDefault(x => x.Id == id); // le retrouve
    }

    // l'officine du pharmacien connecté
    public static Pharmacie? Officine => Courant == null
        ? null
        : Depot.Serveur.Pharmacies.FirstOrDefault(p => p.Id == Courant.PharmacieId);

    // ouvre la session d'un pharmacien
    private static void OuvrirSession(Pharmacien p)
    {
        Courant = p;                                 // mémoire
        Preferences.Set("pharmacien_id", p.Id);      // et sur le téléphone
    }

    // inscrit un pharmacien et crée sa fiche d'officine ; renvoie un message d'erreur ou null si tout va bien
    public static string? Inscrire(DemandeInscription d)
    {
        string email = d.Email.Trim().ToLowerInvariant(); // e-mail normalisé
        var b = Depot.Serveur; // base
        if (b.Pharmaciens.Any(x => x.Email == email)) return "Un compte existe déjà avec cet e-mail"; // doublon

        var centre = Config.CentreQuartier(d.Quartier); // centre du quartier (si pas de GPS)
        int idPharmacie = b.Pharmacies.Count == 0 ? 1 : b.Pharmacies.Max(x => x.Id) + 1; // nouvel identifiant
        b.Pharmacies.Add(new Pharmacie
        {
            Id = idPharmacie, Nom = d.NomOfficine.Trim(), Adresse = d.Adresse.Trim(), Quartier = d.Quartier, Ville = d.Ville,
            Latitude = d.Latitude ?? centre.Latitude, Longitude = d.Longitude ?? centre.Longitude,
            Telephone = d.Telephone.Trim(), Source = "Inscription"
        });

        string sel = Securite.NouveauSel(); // sel du mot de passe
        var pharmacien = new Pharmacien
        {
            Id = b.Pharmaciens.Count == 0 ? 1 : b.Pharmaciens.Max(x => x.Id) + 1,
            PharmacieId = idPharmacie, NomComplet = d.NomComplet.Trim(), Email = email,
            Sel = sel, MotDePasseHash = Securite.Hacher(d.MotDePasse, sel), NumeroOnpc = d.NumeroOnpc.Trim(),
            Statut = Config.ValidationAutomatique ? StatutCompte.Valide : StatutCompte.EnAttente // validation auto en démo
        };
        b.Pharmaciens.Add(pharmacien); // enregistre le compte
        Depot.Sauver();                // écrit le fichier
        OuvrirSession(pharmacien);     // connecte directement
        return null;
    }

    // connecte un pharmacien ; renvoie un message d'erreur ou null
    public static string? Connecter(string email, string motDePasse)
    {
        if (DateTime.UtcNow < _bloqueJusqua) return "Trop de tentatives. Réessayez dans quelques instants."; // blocage actif

        string mail = (email ?? "").Trim().ToLowerInvariant(); // e-mail normalisé
        var p = Depot.Serveur.Pharmaciens.FirstOrDefault(x => x.Email == mail); // compte correspondant
        bool ok = p != null && Securite.Verifier(motDePasse ?? "", p.Sel, p.MotDePasseHash); // mot de passe correct ?
        if (!ok)
        {
            _echecs++; // un échec de plus
            if (_echecs >= 5) { _bloqueJusqua = DateTime.UtcNow.AddSeconds(30); _echecs = 0; } // 5 échecs : blocage 30 s
            return "Adresse e-mail ou mot de passe invalide";
        }
        if (p!.Statut == StatutCompte.Refuse) return "Votre inscription a été refusée. Contactez l'administrateur.";
        _echecs = 0;       // remise à zéro
        OuvrirSession(p);  // connecte
        return null;
    }

    // déconnecte
    public static void Deconnecter()
    {
        Courant = null;                          // plus personne
        Preferences.Remove("pharmacien_id");     // oublie la session
    }

    // un compte existe-t-il pour cet e-mail ?
    public static bool EmailExiste(string email)
        => Depot.Serveur.Pharmaciens.Any(x => x.Email == (email ?? "").Trim().ToLowerInvariant());

    // change le mot de passe d'un compte (récupération)
    public static void ChangerMotDePasse(string email, string nouveau)
    {
        var p = Depot.Serveur.Pharmaciens.FirstOrDefault(x => x.Email == (email ?? "").Trim().ToLowerInvariant());
        if (p == null) return; // compte introuvable
        p.Sel = Securite.NouveauSel();                   // nouveau sel
        p.MotDePasseHash = Securite.Hacher(nouveau, p.Sel); // nouvelle empreinte
        Depot.Sauver();                                  // enregistre
    }

    // démonstration : valide le compte connecté (à la place de l'administrateur)
    public static void ValiderCourant()
    {
        if (Courant == null) return;
        Courant.Statut = StatutCompte.Valide; // compte validé
        Depot.Sauver();
    }
}