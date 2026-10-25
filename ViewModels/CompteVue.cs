using System.Windows.Input;
using PharmaGarde.Helpers;
using PharmaGarde.Models;

namespace PharmaGarde.ViewModels;

// Un compte pharmacien prêt à afficher dans l'écran d'administration
public class CompteVue
{
    public Pharmacien Compte { get; }   // le compte
    public Pharmacie? Officine { get; } // son officine

    // textes affichés
    public string NomOfficine => Officine?.Nom ?? "(officine supprimée)";
    public string Titulaire => Compte.NomComplet;
    public string Onpc => $"N° ONPC : {Compte.NumeroOnpc}";
    public string Email => Compte.Email;
    public string DateTexte => $"Inscrit le {Heure.VersLocal(Compte.CreeLe):dd/MM/yyyy}";

    // badge de statut
    public string BadgeTexte => Compte.Statut switch
    {
        StatutCompte.EnAttente => "● En attente",
        StatutCompte.Valide => "● Validé",
        StatutCompte.Suspendu => "● Suspendu",
        _ => "● Refusé"
    };
    public Color BadgeFond => Color.FromArgb(Compte.Statut switch
    {
        StatutCompte.EnAttente => "#3D2F10",
        StatutCompte.Valide => "#1B3A26",
        _ => "#3B1414"
    });
    public Color BadgeCouleur => Color.FromArgb(Compte.Statut switch
    {
        StatutCompte.EnAttente => "#FFB74D",
        StatutCompte.Valide => "#3DDC6F",
        _ => "#FF8A80"
    });

    // les deux boutons d'action de la carte
    public string Libelle1 { get; } = "";
    public string Icone1 { get; } = "";
    public string Variante1 { get; } = "Primary";
    public ICommand Commande1 { get; }
    public string Libelle2 { get; } = "";
    public string Icone2 { get; } = "";
    public string Variante2 { get; } = "Secondary";
    public ICommand Commande2 { get; }

    // "executer" est la fonction du ViewModel qui réalise l'action ("valider", "refuser"...)
    public CompteVue(Pharmacien compte, Pharmacie? officine, Func<Pharmacien, string, Task> executer)
    {
        Compte = compte;
        Officine = officine;

        // actions proposées selon le statut du compte
        string a1, a2; // identifiants des deux actions
        switch (compte.Statut)
        {
            case StatutCompte.EnAttente:
                Libelle1 = "Valider"; Icone1 = Icons.Check; Variante1 = "Primary"; a1 = "valider";
                Libelle2 = "Refuser"; Icone2 = Icons.Close; Variante2 = "Danger"; a2 = "refuser";
                break;
            case StatutCompte.Valide:
                Libelle1 = "Suspendre"; Icone1 = Icons.Lock; Variante1 = "Secondary"; a1 = "suspendre";
                Libelle2 = "Supprimer"; Icone2 = Icons.Delete; Variante2 = "Danger"; a2 = "supprimer";
                break;
            case StatutCompte.Suspendu:
                Libelle1 = "Réactiver"; Icone1 = Icons.Check; Variante1 = "Primary"; a1 = "reactiver";
                Libelle2 = "Supprimer"; Icone2 = Icons.Delete; Variante2 = "Danger"; a2 = "supprimer";
                break;
            default: // refusé : on peut revenir sur la décision ou supprimer
                Libelle1 = "Valider"; Icone1 = Icons.Check; Variante1 = "Primary"; a1 = "valider";
                Libelle2 = "Supprimer"; Icone2 = Icons.Delete; Variante2 = "Danger"; a2 = "supprimer";
                break;
        }
        Commande1 = new RelayCommand(() => executer(compte, a1)); // commande du 1er bouton
        Commande2 = new RelayCommand(() => executer(compte, a2)); // commande du 2e bouton
    }
}