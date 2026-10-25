namespace PharmaGarde;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent(); // charge AppShell.xaml
        // pages "poussées" : GoToAsync("Nom") pour ouvrir, GoToAsync("..") pour revenir
        Routing.RegisterRoute("Detail", typeof(Views.DetailPage));                     // fiche d'une pharmacie
        Routing.RegisterRoute("Legal", typeof(Views.LegalPage));                       // informations légales
        Routing.RegisterRoute("Permanence", typeof(Views.PermanencePage));             // formulaire de permanence
        Routing.RegisterRoute("MotDePasseOublie", typeof(Views.MotDePasseOubliePage)); // récupération 1/3
        Routing.RegisterRoute("CodeVerification", typeof(Views.CodeVerificationPage)); // récupération 2/3
        Routing.RegisterRoute("NouveauMdp", typeof(Views.NouveauMdpPage));             // récupération 3/3
    }
}
