namespace PharmaGarde.Controls;

public partial class ChampChoix : ContentView
{
    public static readonly BindableProperty LibelleProperty = BindableProperty.Create(nameof(Libelle), typeof(string), typeof(ChampChoix), "",
        propertyChanged: (b, o, n) => { var c = (ChampChoix)b; c.LblLibelle.Text = (string)n; c.LblLibelle.IsVisible = !string.IsNullOrEmpty((string)n); });

    public static readonly BindableProperty IconeProperty = BindableProperty.Create(nameof(Icone), typeof(string), typeof(ChampChoix), "",
        propertyChanged: (b, o, n) => ((ChampChoix)b).LblIcone.Text = (string)n);

    public static readonly BindableProperty ValeurProperty = BindableProperty.Create(nameof(Valeur), typeof(string), typeof(ChampChoix), "", BindingMode.TwoWay,
        propertyChanged: (b, o, n) => ((ChampChoix)b).MajValeur());

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ChampChoix), "Sélectionner…",
        propertyChanged: (b, o, n) => ((ChampChoix)b).MajValeur());

    // options séparées par "|" : "Akwa|Bonanjo|Deïdo"
    public static readonly BindableProperty ChoixProperty = BindableProperty.Create(nameof(Choix), typeof(string), typeof(ChampChoix), "");

    public static readonly BindableProperty MessageErreurProperty = BindableProperty.Create(nameof(MessageErreur), typeof(string), typeof(ChampChoix), "",
        propertyChanged: (b, o, n) => ((ChampChoix)b).MajErreur());

    public string Libelle { get => (string)GetValue(LibelleProperty); set => SetValue(LibelleProperty, value); }
    public string Icone { get => (string)GetValue(IconeProperty); set => SetValue(IconeProperty, value); }
    public string Valeur { get => (string)GetValue(ValeurProperty); set => SetValue(ValeurProperty, value); }
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public string Choix { get => (string)GetValue(ChoixProperty); set => SetValue(ChoixProperty, value); }
    public string MessageErreur { get => (string)GetValue(MessageErreurProperty); set => SetValue(MessageErreurProperty, value); }

    public event EventHandler? ValeurChangee; // déclenché quand l'utilisateur choisit une option

    public ChampChoix()
    {
        InitializeComponent(); // charge le XAML
        MajValeur();           // affiche le texte d'invite
    }

    // affiche la valeur choisie ou le texte d'invite
    private void MajValeur()
    {
        bool vide = string.IsNullOrEmpty(Valeur); // rien de choisi ?
        LblValeur.Text = vide ? Placeholder : Valeur;
        LblValeur.TextColor = Color.FromArgb(vide ? "#6B6B6B" : "#EDEDED"); // gris si vide
    }

    // style normal ou erreur
    private void MajErreur()
    {
        bool erreur = !string.IsNullOrEmpty(MessageErreur);
        Cadre.Stroke = new SolidColorBrush(erreur ? Color.FromArgb("#FF8A80") : Colors.Transparent);
        Cadre.BackgroundColor = Color.FromArgb(erreur ? "#2A1616" : "#232323");
        LblMessage.Text = MessageErreur;
        LblMessage.IsVisible = erreur;
    }

    // toucher : ouvre la boîte de choix
    private async void OnTape(object? sender, TappedEventArgs e)
    {
        var options = Choix.Split('|', StringSplitOptions.RemoveEmptyEntries); // liste des options
        if (options.Length == 0) return; // rien à proposer
        string? choix = await Shell.Current.DisplayActionSheet(string.IsNullOrEmpty(Libelle) ? "Choisir" : Libelle, "Annuler", null, options);
        if (string.IsNullOrEmpty(choix) || choix == "Annuler") return; // annulé
        Valeur = choix;           // enregistre le choix
        MessageErreur = "";       // efface l'erreur
        ValeurChangee?.Invoke(this, EventArgs.Empty); // prévient la page
    }
}