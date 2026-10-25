using PharmaGarde.Helpers;  // Outils
using PharmaGarde.Services; // AuthService

namespace PharmaGarde;

public partial class App : Application
{
    public App()
    {
        InitializeComponent(); // charge App.xaml
        UserAppTheme = AppTheme.Dark; // thème sombre imposé
        Outils.AppliquerThemeOled(Preferences.Get("oled", true)); // fond noir pur si le mode OLED est actif
        AuthService.RestaurerSession(); // retrouve le pharmacien connecté à la session précédente
    }

    // crée la fenêtre principale avec le Shell (navigation)
    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new AppShell());
}