using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Helpers;

public static class GardePharmacien
{
    // renvoie vrai si l'écran peut s'afficher ; sinon redirige vers la bonne page et renvoie faux
    public static async Task<bool> AutoriseAsync()
    {
        var p = AuthService.Courant; // personne connectée ?
        if (p == null) { await Outils.Aller("//Connexion"); return false; } // personne : connexion

        if (p.Role == RoleCompte.Admin) { await Outils.Aller("//Admin"); return false; } // un administrateur n'a pas d'officine

        if (p.Statut == StatutCompte.Suspendu || p.Statut == StatutCompte.Refuse) // compte bloqué
        {
            string message = p.Statut == StatutCompte.Suspendu
                ? "Votre compte a été suspendu. Contactez l'administrateur."
                : "Votre inscription a été refusée. Contactez l'administrateur.";
            await Shell.Current.DisplayAlert("Accès impossible", message, "OK");
            AuthService.Deconnecter();
            await Outils.Aller("//Connexion");
            return false;
        }

        if (p.Statut != StatutCompte.Valide) { await Outils.Aller("//Attente"); return false; } // pas encore validé
        return true; // tout est bon
    }
}