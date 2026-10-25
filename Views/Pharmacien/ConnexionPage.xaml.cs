using PharmaGarde.ViewModels;

namespace PharmaGarde.Views;

public partial class ConnexionPage : ContentPage
{
    private readonly ConnexionViewModel _vm = new(); // le ViewModel de cette page

    public ConnexionPage()
    {
        InitializeComponent(); // charge le XAML
        BindingContext = _vm;  // relie la Vue à son ViewModel : c'est le coeur du MVVM
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();   // comportement standard
        _vm.Reinitialiser();  // efface le mot de passe et les erreurs
    }
}