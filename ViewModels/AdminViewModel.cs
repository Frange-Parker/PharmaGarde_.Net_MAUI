using System.Collections.ObjectModel;
using System.Windows.Input;
using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.ViewModels;

public class AdminViewModel : BaseViewModel
{
    private string _filtre = "attente"; // liste affichée : attente, valides ou autres
    private ObservableCollection<CompteVue> _comptes = new();
    private bool _vide;
    private string _messageVide = "";
    private string _texteAttente = "Attente", _texteValides = "Validés", _texteAutres = "Autres";
    private string _varianteAttente = "Primary", _varianteValides = "Secondary", _varianteAutres = "Secondary";

    // données lues par la Vue
    public ObservableCollection<CompteVue> Comptes { get => _comptes; private set => SetProperty(ref _comptes, value); }
    public bool Vide { get => _vide; private set => SetProperty(ref _vide, value); }
    public string MessageVide { get => _messageVide; private set => SetProperty(ref _messageVide, value); }
    public string TexteAttente { get => _texteAttente; private set => SetProperty(ref _texteAttente, value); }
    public string TexteValides { get => _texteValides; private set => SetProperty(ref _texteValides, value); }
    public string TexteAutres { get => _texteAutres; private set => SetProperty(ref _texteAutres, value); }
    public string VarianteAttente { get => _varianteAttente; private set => SetProperty(ref _varianteAttente, value); }
    public string VarianteValides { get => _varianteValides; private set => SetProperty(ref _varianteValides, value); }
    public string VarianteAutres { get => _varianteAutres; private set => SetProperty(ref _varianteAutres, value); }

    // commandes déclenchées par la Vue
    public ICommand FiltreAttenteCommand { get; }
    public ICommand FiltreValidesCommand { get; }
    public ICommand FiltreAutresCommand { get; }
    public ICommand DeconnexionCommand { get; }

    public AdminViewModel()
    {
        FiltreAttenteCommand = new RelayCommand(() => ChoisirFiltre("attente"));
        FiltreValidesCommand = new RelayCommand(() => ChoisirFiltre("valides"));
        FiltreAutresCommand = new RelayCommand(() => ChoisirFiltre("autres"));
        DeconnexionCommand = new RelayCommand(async () =>
        {
            AuthService.Deconnecter();          // ferme la session administrateur
            await Outils.Aller("//Accueil");    // retour à l'application publique
        });
    }

    // appelée à chaque affichage de l'écran
    public void Charger() => Rafraichir();

    private Task ChoisirFiltre(string filtre)
    {
        _filtre = filtre; // mémorise la liste choisie
        Rafraichir();
        return Task.CompletedTask;
    }

    // relit les comptes et reconstruit l'affichage
    private void Rafraichir()
    {
        var tous = AdminService.Comptes();
        int nAttente = tous.Count(x => x.Statut == StatutCompte.EnAttente);
        int nValides = tous.Count(x => x.Statut == StatutCompte.Valide);
        int nAutres = tous.Count - nAttente - nValides; // refusés + suspendus

        TexteAttente = $"Attente {nAttente}";
        TexteValides = $"Validés {nValides}";
        TexteAutres = $"Autres {nAutres}";
        VarianteAttente = _filtre == "attente" ? "Primary" : "Secondary"; // le bouton actif est vert
        VarianteValides = _filtre == "valides" ? "Primary" : "Secondary";
        VarianteAutres = _filtre == "autres" ? "Primary" : "Secondary";

        IEnumerable<Pharmacien> choisis = _filtre switch
        {
            "attente" => tous.Where(x => x.Statut == StatutCompte.EnAttente),
            "valides" => tous.Where(x => x.Statut == StatutCompte.Valide),
            _ => tous.Where(x => x.Statut == StatutCompte.Refuse || x.Statut == StatutCompte.Suspendu)
        };
        Comptes = new ObservableCollection<CompteVue>(choisis.Select(c => new CompteVue(c, AdminService.OfficineDe(c), Executer)));

        Vide = Comptes.Count == 0; // état vide
        MessageVide = _filtre switch
        {
            "attente" => "Aucune inscription en attente de vérification.",
            "valides" => "Aucun compte validé.",
            _ => "Aucun compte refusé ou suspendu."
        };
    }

    // réalise l'action choisie sur un compte
    private async Task Executer(Pharmacien compte, string action)
    {
        string nom = AdminService.OfficineDe(compte)?.Nom ?? compte.NomComplet; // nom affiché dans les messages
        switch (action)
        {
            case "valider":
            case "reactiver":
                AdminService.ChangerStatut(compte.Id, StatutCompte.Valide); // le compte peut publier
                break;
            case "refuser":
                if (!await Confirmer("Refuser cette inscription ?", $"{nom} ne pourra pas publier de permanences.", "Refuser")) return;
                AdminService.ChangerStatut(compte.Id, StatutCompte.Refuse);
                break;
            case "suspendre":
                if (!await Confirmer("Suspendre ce compte ?", $"Les gardes de {nom} disparaîtront de l'application publique.", "Suspendre")) return;
                AdminService.ChangerStatut(compte.Id, StatutCompte.Suspendu);
                break;
            case "supprimer":
                if (!await Confirmer("Supprimer définitivement ?", $"Le compte et les permanences de {nom} seront effacés.", "Supprimer")) return;
                AdminService.Supprimer(compte.Id);
                break;
        }
        Rafraichir(); // met l'écran à jour
    }

    // fenêtre de confirmation
    private static Task<bool> Confirmer(string titre, string message, string bouton)
        => Shell.Current.DisplayAlert(titre, message, bouton, "Annuler");
}