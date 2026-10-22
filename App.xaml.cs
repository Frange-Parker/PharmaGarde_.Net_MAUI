namespace PharmaGarde;

public partial class App : Application
{
    public App()
    {
        InitializeComponent(); // charge App.xaml
        UserAppTheme = AppTheme.Dark; // thème sombre imposé (écoconception)
    }

    // crée la fenêtre principale avec le Shell (navigation)
    protected override Window CreateWindow(IActivationState? activationState)
        => new Window(new AppShell()); // (en .NET 8 : MainPage = new AppShell(); dans le constructeur)
}