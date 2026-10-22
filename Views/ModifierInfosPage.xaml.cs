namespace PharmaGarde.Views;

public partial class ModifierInfosPage : ContentPage
{
    public ModifierInfosPage()
    {
        InitializeComponent(); // charge le XAML
        // pré-remplit avec les valeurs enregistrées (ou celles de la maquette)
        Nom.Text = Preferences.Get("nom", "Dr. JeanPaul Mbarga");
        Email.Text = Preferences.Get("email", "jp.mbarga@sante.cm");
        Tel.Text = Preferences.Get("tel", "+237 699 45 82 10");
        TxtVille.Text = Preferences.Get("ville", "Douala");
        TxtQuartier.Text = Preferences.Get("quartier", "Akwa");
    }

    // boîte de sélection de la ville
    private async void OnChoisirVille(object? sender, TappedEventArgs e)
    {
        string? choix = await DisplayActionSheet("Ville de couverture", "Annuler", null, "Douala", "Yaoundé");
        if (choix != null && choix != "Annuler") TxtVille.Text = choix; // met à jour si choix valide
    }

    // boîte de sélection du quartier
    private async void OnChoisirQuartier(object? sender, TappedEventArgs e)
    {
        string? choix = await DisplayActionSheet("Quartier habituel", "Annuler", null,
            "Akwa", "Bonanjo", "Bonapriso", "Deïdo", "Bali", "Bépanda");
        if (choix != null && choix != "Annuler") TxtQuartier.Text = choix;
    }

    // dès qu'on retape le nom, l'erreur disparaît
    private void OnTexteModifie(object? sender, TextChangedEventArgs e)
    {
        ErrNom.IsVisible = false;
        ChampNom.Stroke = new SolidColorBrush(Colors.Transparent);
    }

    private async void OnEnregistrer(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(Nom.Text)) // le nom est obligatoire
        {
            ErrNom.IsVisible = true;
            ChampNom.Stroke = new SolidColorBrush(Color.FromArgb("#FF8A80"));
            return;
        }
        Preferences.Set("nom", Nom.Text.Trim());   // enregistre sur le téléphone
        Preferences.Set("email", Email.Text?.Trim() ?? "");
        Preferences.Set("tel", Tel.Text?.Trim() ?? "");
        Preferences.Set("ville", TxtVille.Text);
        Preferences.Set("quartier", TxtQuartier.Text);
        await DisplayAlert("Enregistré", "Vos informations ont été mises à jour.", "OK");
        await Shell.Current.GoToAsync(".."); // retour au profil
    }

    private async void OnAnnuler(object? sender, EventArgs e) => await Shell.Current.GoToAsync("..");
    private async void OnRetour(object? sender, TappedEventArgs e) => await Shell.Current.GoToAsync("..");
}