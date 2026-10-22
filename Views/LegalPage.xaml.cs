namespace PharmaGarde.Views;

public partial class LegalPage : ContentPage
{
    public LegalPage()
    {
        InitializeComponent(); // charge le XAML
    }

    // retour à l'écran précédent (les paramètres)
    private async void OnRetour(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync("..");
}