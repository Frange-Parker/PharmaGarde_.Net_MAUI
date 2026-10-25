namespace PharmaGarde.Models;

// statut affiché pour une pharmacie (règle RG2)
public enum StatutGarde { DeGarde, ProchaineGarde, InformationAncienne, AucuneDonnee }

// type de permanence déclarée par le pharmacien
public enum TypePermanence { Nuit, WeekEnd, JourFerie, Continu24h }

// état d'un compte (Suspendu est ajouté à la fin pour ne pas changer les valeurs déjà enregistrées)
public enum StatutCompte { EnAttente, Valide, Refuse, Suspendu }

// rôle d'un compte : pharmacien ou administrateur
public enum RoleCompte { Pharmacien, Admin }

// textes d'affichage des types de permanence
public static class Libelles
{
    public const string ChoixTypes = "Nuit|Week-end|Jour férié|24h/24"; // pour les listes de choix

    // type -> texte
    public static string Type(TypePermanence t) => t switch
    {
        TypePermanence.Nuit => "Nuit",
        TypePermanence.WeekEnd => "Week-end",
        TypePermanence.JourFerie => "Jour férié",
        _ => "24h/24"
    };

    // texte -> type
    public static TypePermanence TypeDepuis(string s) => s switch
    {
        "Week-end" => TypePermanence.WeekEnd,
        "Jour férié" => TypePermanence.JourFerie,
        "24h/24" => TypePermanence.Continu24h,
        _ => TypePermanence.Nuit
    };
}