using PharmaGarde.Helpers;
using PharmaGarde.Services;
using PharmaGarde.ViewModels;

namespace PharmaGarde.Views;

public partial class AdminPage : ContentPage
{
    private readonly AdminViewModel _vm = new(); // le ViewModel de cette page

    public AdminPage()
    {
        InitializeComponent(); // charge le XAML
        BindingContext = _vm;  // relie la Vue à son ViewModel
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        if (!AdminService.EstAdminConnecte) { await Outils.Aller("//Connexion"); return; } // réservé à l'administrateur
        _vm.Charger(); // affiche les comptes
    }
}