using PharmaGarde.Helpers;

namespace PharmaGarde.Controls;

public partial class ChampSaisie : ContentView
{
    // ---------- propriétés utilisables en XAML ----------
    public static readonly BindableProperty LibelleProperty = BindableProperty.Create(nameof(Libelle), typeof(string), typeof(ChampSaisie), "",
        propertyChanged: (b, o, n) => { var c = (ChampSaisie)b; c.LblLibelle.Text = (string)n; c.LblLibelle.IsVisible = !string.IsNullOrEmpty((string)n); });

    public static readonly BindableProperty IconeProperty = BindableProperty.Create(nameof(Icone), typeof(string), typeof(ChampSaisie), "",
        propertyChanged: (b, o, n) => { var c = (ChampSaisie)b; c.LblIcone.Text = (string)n; c.LblIcone.IsVisible = !string.IsNullOrEmpty((string)n); });

    public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(nameof(Placeholder), typeof(string), typeof(ChampSaisie), "",
        propertyChanged: (b, o, n) => ((ChampSaisie)b).Saisie.Placeholder = (string)n);

    public static readonly BindableProperty TexteProperty = BindableProperty.Create(nameof(Texte), typeof(string), typeof(ChampSaisie), "", BindingMode.TwoWay,
        propertyChanged: (b, o, n) => { var c = (ChampSaisie)b; string v = (string?)n ?? ""; if (c.Saisie.Text != v) c.Saisie.Text = v; });

    public static readonly BindableProperty EstMotDePasseProperty = BindableProperty.Create(nameof(EstMotDePasse), typeof(bool), typeof(ChampSaisie), false,
        propertyChanged: (b, o, n) => { var c = (ChampSaisie)b; bool v = (bool)n; c.Saisie.IsPassword = v; c.LblOeil.IsVisible = v; c.LblOeil.Text = Icons.EyeOff; });

    public static readonly BindableProperty ClavierProperty = BindableProperty.Create(nameof(Clavier), typeof(string), typeof(ChampSaisie), "Text",
        propertyChanged: (b, o, n) => ((ChampSaisie)b).Saisie.Keyboard = (string)n switch
        {
            "Email" => Keyboard.Email,
            "Telephone" => Keyboard.Telephone,
            "Numeric" => Keyboard.Numeric,
            _ => Keyboard.Default
        });

    public static readonly BindableProperty LongueurMaxProperty = BindableProperty.Create(nameof(LongueurMax), typeof(int), typeof(ChampSaisie), 0,
        propertyChanged: (b, o, n) => { if ((int)n > 0) ((ChampSaisie)b).Saisie.MaxLength = (int)n; });

    public static readonly BindableProperty MessageErreurProperty = BindableProperty.Create(nameof(MessageErreur), typeof(string), typeof(ChampSaisie), "",
        propertyChanged: (b, o, n) => ((ChampSaisie)b).MajErreur());

    public static readonly BindableProperty EnErreurProperty = BindableProperty.Create(nameof(EnErreur), typeof(bool), typeof(ChampSaisie), false,
        propertyChanged: (b, o, n) => ((ChampSaisie)b).MajErreur());

    public string Libelle { get => (string)GetValue(LibelleProperty); set => SetValue(LibelleProperty, value); }
    public string Icone { get => (string)GetValue(IconeProperty); set => SetValue(IconeProperty, value); }
    public string Placeholder { get => (string)GetValue(PlaceholderProperty); set => SetValue(PlaceholderProperty, value); }
    public string Texte { get => (string)GetValue(TexteProperty); set => SetValue(TexteProperty, value); }
    public bool EstMotDePasse { get => (bool)GetValue(EstMotDePasseProperty); set => SetValue(EstMotDePasseProperty, value); }
    public string Clavier { get => (string)GetValue(ClavierProperty); set => SetValue(ClavierProperty, value); }
    public int LongueurMax { get => (int)GetValue(LongueurMaxProperty); set => SetValue(LongueurMaxProperty, value); }
    public string MessageErreur { get => (string)GetValue(MessageErreurProperty); set => SetValue(MessageErreurProperty, value); }
    public bool EnErreur { get => (bool)GetValue(EnErreurProperty); set => SetValue(EnErreurProperty, value); }

    public event EventHandler? TexteModifie; // déclenché à chaque frappe

    public ChampSaisie()
    {
        InitializeComponent(); // charge le XAML
    }

    // met le focus dans le champ
    public void Focaliser() => Saisie.Focus();

    // style normal ou erreur
    private void MajErreur()
    {
        bool erreur = EnErreur || !string.IsNullOrEmpty(MessageErreur); // y a-t-il une erreur ?
        Cadre.Stroke = new SolidColorBrush(erreur ? Color.FromArgb("#FF8A80") : Colors.Transparent); // bordure rouge
        Cadre.BackgroundColor = Color.FromArgb(erreur ? "#2A1616" : "#232323"); // fond rougeâtre
        LblMessage.Text = MessageErreur;                          // texte de l'erreur
        LblMessage.IsVisible = !string.IsNullOrEmpty(MessageErreur); // visible s'il y a un message
    }

    // à chaque frappe : mémorise le texte et retire l'erreur
    private void OnTexteModifie(object? sender, TextChangedEventArgs e)
    {
        Texte = e.NewTextValue ?? ""; // met à jour la propriété
        if (EnErreur || !string.IsNullOrEmpty(MessageErreur)) { EnErreur = false; MessageErreur = ""; } // efface l'erreur
        TexteModifie?.Invoke(this, EventArgs.Empty); // prévient la page
    }

    // œil : affiche ou masque le mot de passe
    private void OnOeil(object? sender, TappedEventArgs e)
    {
        Saisie.IsPassword = !Saisie.IsPassword; // inverse
        LblOeil.Text = Saisie.IsPassword ? Icons.EyeOff : Icons.Eye; // change l'icône
    }
}