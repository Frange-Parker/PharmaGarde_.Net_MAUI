using PharmaGarde.Helpers;
using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Views;

public partial class AttentePage : ContentPage
{
    public AttentePage()
    {
        InitializeComponent(); // charge le XAML
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing(); // comportement standard
        var p = AuthService.Courant;
        if (p == null) { await Outils.Aller("//Connexion"); return; }                       // personne de connecté
        if (p.Statut == StatutCompte.Valide) { await Outils.Aller("//Tableau"); return; }   // déjà validé
        BtnDemo.IsVisible = Config.ModeDemo; // bouton de démonstration
    }

    private async void OnRetourRecherche(object? sender, EventArgs e) => await Outils.Aller("//Accueil");

    // démonstration : valide le compte comme le ferait l'administrateur
    private async void OnSimuler(object? sender, EventArgs e)
    {
        AuthService.ValiderCourant();
        await Outils.Aller("//Tableau");
    }

    private async void OnDeconnexion(object? sender, TappedEventArgs e)
    {
        AuthService.Deconnecter();
        await Outils.Aller("//Accueil");
    }
}