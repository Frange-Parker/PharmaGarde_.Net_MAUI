using PharmaGarde.Helpers;
using PharmaGarde.Models;

namespace PharmaGarde.Services;

public static class CalculStatut
{
    // renvoie : le statut, la permanence retenue, et si l'information est plus ancienne que le seuil
    public static (StatutGarde Statut, Permanence? Permanence, bool Ancienne) Determiner(List<Permanence> permanences, DateTime maintenantUtc)
    {
        // 1) une permanence en cours ?
        var enCours = permanences.Where(x => x.DebutUtc <= maintenantUtc && maintenantUtc < x.FinUtc)
            .OrderBy(x => x.FinUtc).FirstOrDefault();
        if (enCours != null) return (StatutGarde.DeGarde, enCours, false);

        // 2) sinon la prochaine à venir
        var prochaine = permanences.Where(x => x.DebutUtc > maintenantUtc)
            .OrderBy(x => x.DebutUtc).FirstOrDefault();
        if (prochaine != null) return (StatutGarde.ProchaineGarde, prochaine, false);

        // 3) sinon la dernière connue (information ancienne)
        var derniere = permanences.OrderByDescending(x => x.FinUtc).FirstOrDefault();
        if (derniere != null)
            return (StatutGarde.InformationAncienne, derniere, (maintenantUtc - derniere.FinUtc).TotalDays > Config.SeuilAncienJours);

        // 4) rien du tout
        return (StatutGarde.AucuneDonnee, null, false);
    }
}