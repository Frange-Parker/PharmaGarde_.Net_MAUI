using System.Windows.Input;
using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.ViewModels;

public class ConnexionViewModel : BaseViewModel
{
    private string _email = "";            // e-mail saisi
    private string _motDePasse = "";       // mot de passe saisi
    private bool _emailEnErreur;           // bordure rouge sur l'e-mail ?
    private string _erreurMotDePasse = ""; // message sous le mot de passe
    private bool _boutonActif = true;      // bouton "Se connecter" actif ?

    // saisies : toute modification efface l'erreur
    public string Email { get => _email; set { if (SetProperty(ref _email, value)) EffacerErreurs(); } }
    public string MotDePasse { get => _motDePasse; set { if (SetProperty(ref _motDePasse, value)) EffacerErreurs(); } }

    // état d'erreur lu par la Vue
    public bool EmailEnErreur { get => _emailEnErreur; set => SetProperty(ref _emailEnErreur, value); }
    public string ErreurMotDePasse { get => _erreurMotDePasse; set => SetProperty(ref _erreurMotDePasse, value); }
    public bool BoutonActif { get => _boutonActif; set => SetProperty(ref _boutonActif, value); }
    public bool DemoVisible => Config.ModeDemo; // rappel des identifiants de démonstration

    // commandes déclenchées par la Vue
    public ICommand ConnecterCommand { get; }
    public ICommand MdpOublieCommand { get; }
    public ICommand RetourRechercheCommand { get; }
    public ICommand InscriptionCommand { get; }

    public ConnexionViewModel()
    {
        ConnecterCommand = new RelayCommand(Connecter);
        MdpOublieCommand = new RelayCommand(() => Outils.Aller("MotDePasseOublie"));
        RetourRechercheCommand = new RelayCommand(() => Outils.Aller("//Accueil"));
        InscriptionCommand = new RelayCommand(() => Outils.Aller("//Inscription"));
    }

    // vérifie les identifiants puis navigue selon le rôle et le statut du compte
    private async Task Connecter()
    {
        AdminService.AssurerCompteAdmin(); // le compte administrateur existe toujours

        string? erreur = AuthService.Connecter(Email, MotDePasse); // appelle le service
        if (erreur != null)
        {
            EmailEnErreur = true;      // bordure rouge sur l'e-mail
            ErreurMotDePasse = erreur; // message sous le mot de passe
            BoutonActif = false;       // bouton désactivé tant que l'erreur est là
            return;
        }

        var compte = AuthService.Courant!; // compte connecté

        if (compte.Role == RoleCompte.Admin) // administrateur : écran d'administration
        {
            await Outils.Aller("//Admin");
            return;
        }

        if (compte.Statut == StatutCompte.Suspendu) // compte suspendu : accès refusé
        {
            AuthService.Deconnecter();
            EmailEnErreur = true;
            ErreurMotDePasse = "Votre compte a été suspendu. Contactez l'administrateur.";
            BoutonActif = false;
            return;
        }

        // pharmacien : tableau de bord si le compte est validé, sinon écran d'attente
        await Outils.Aller(compte.Statut == StatutCompte.Valide ? "//Tableau" : "//Attente");
    }

    private void EffacerErreurs()
    {
        EmailEnErreur = false;
        ErreurMotDePasse = "";
        BoutonActif = true;
    }

    // appelée à chaque affichage de l'écran
    public void Reinitialiser()
    {
        MotDePasse = ""; // efface l'ancien mot de passe
        EffacerErreurs();
    }
}