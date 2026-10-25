using PharmaGarde.Helpers;

namespace PharmaGarde.Views;

public partial class SplashPage : ContentPage
{
    public SplashPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // appelée quand l'écran s'affiche
    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        await Task.Delay(2200); // laisse voir l'écran de lancement
        await Outils.Aller("//Accueil"); // va à l'accueil (aucun compte nécessaire)
    }
}