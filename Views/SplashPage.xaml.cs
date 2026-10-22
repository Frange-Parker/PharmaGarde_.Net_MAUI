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
        await Task.Delay(2500); // attend 2,5 secondes (simule l'actualisation)
        await Shell.Current.GoToAsync("//Connexion"); // passe à l'écran de connexion
    }
}