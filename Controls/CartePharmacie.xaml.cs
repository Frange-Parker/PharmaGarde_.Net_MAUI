using PharmaGarde.Helpers;
using PharmaGarde.Models;

namespace PharmaGarde.Controls;

public partial class CartePharmacie : ContentView
{
    public CartePharmacie()
    {
        InitializeComponent(); // charge le XAML
    }

    // la pharmacie affichée par cette carte (fournie par la liste)
    private PharmacieVue? Vue => BindingContext as PharmacieVue;

    // ouvre la fiche de cette pharmacie
    private async void OnOuvrirDetail(object? sender, TappedEventArgs e)
    {
        if (Vue != null) await Outils.Aller($"Detail?id={Vue.Id}&horsligne={!Vue.ItineraireActif}");
    }

    // appelle la pharmacie
    private async void OnAppeler(object? sender, EventArgs e)
    {
        if (Vue != null) await Outils.Appeler(Vue.Telephone);
    }

    // lance le GPS vers la pharmacie
    private async void OnItineraire(object? sender, EventArgs e)
    {
        if (Vue != null) await Outils.Itineraire(Vue.Latitude, Vue.Longitude, Vue.Nom);
    }
}