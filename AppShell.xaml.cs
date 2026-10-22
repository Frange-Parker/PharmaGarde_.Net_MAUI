namespace PharmaGarde;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent(); // charge AppShell.xaml
        
        // pages ouvertes par-dessus les autres avec GoToAsync("nom") ; ".." pour revenir
        Routing.RegisterRoute("Detail", typeof(Views.DetailPage));                     // fiche pharmacie
        Routing.RegisterRoute("Parametres", typeof(Views.ParametresPage));             // paramètres
        Routing.RegisterRoute("MotDePasseOublie", typeof(Views.MotDePasseOubliePage)); // mot de passe oublié (1/3)
        Routing.RegisterRoute("CodeVerification", typeof(Views.CodeVerificationPage)); // code reçu (2/3)
        Routing.RegisterRoute("NouveauMdp", typeof(Views.NouveauMdpPage));             // nouveau mot de passe (3/3)
    }
}
 