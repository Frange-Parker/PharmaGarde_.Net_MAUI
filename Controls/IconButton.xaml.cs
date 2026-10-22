namespace PharmaGarde.Controls;

public partial class IconButton : ContentView
{
    // propriété "Text" utilisable en XAML : Text="Appeler"
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(IconButton), string.Empty,
        propertyChanged: (b, o, n) => ((IconButton)b).Actualiser()); // redessine quand ça change
    // propriété "Icon" : code de l'icône
    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon), typeof(string), typeof(IconButton), string.Empty,
        propertyChanged: (b, o, n) => ((IconButton)b).Actualiser());
    // propriété "Variant" : Primary, Secondary ou Danger
    public static readonly BindableProperty VariantProperty = BindableProperty.Create(
        nameof(Variant), typeof(string), typeof(IconButton), "Primary",
        propertyChanged: (b, o, n) => ((IconButton)b).Actualiser());

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public string Variant { get => (string)GetValue(VariantProperty); set => SetValue(VariantProperty, value); }

    public event EventHandler? Clicked; // événement déclenché au toucher

    public IconButton()
    {
        InitializeComponent(); // charge le XAML
        Actualiser(); // applique les couleurs initiales
    }

    // redessine le bouton quand IsEnabled change (état désactivé)
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName); // comportement standard
        if (propertyName == nameof(IsEnabled)) Actualiser(); // met à jour les couleurs
    }

    // choisit les couleurs selon la variante et l'état
    private void Actualiser()
    {
        LabelTexte.Text = Text; // affiche le texte
        LabelIcone.Text = Icon; // affiche l'icône
        LabelIcone.IsVisible = !string.IsNullOrEmpty(Icon); // cache l'icône si absente
        LabelTexte.IsVisible = !string.IsNullOrEmpty(Text); // cache le texte si absent

        // (fond, couleur du texte, couleur de l'icône)
        var (fond, texte, icone) = !IsEnabled
            ? ("#2A2A2A", "#6B6B6B", "#6B6B6B")                 // désactivé : gris
            : Variant switch
            {
                "Secondary" => ("#2A2A2A", "#EDEDED", "#3DDC6F"), // secondaire : gris, icône verte
                "Danger" => ("#3B1414", "#FF8A80", "#FF8A80"),    // danger : rouge sombre
                _ => ("#00C853", "#0B1F10", "#0B1F10")            // principal : vert
            };

        Boite.BackgroundColor = Color.FromArgb(fond); // applique le fond
        LabelTexte.TextColor = Color.FromArgb(texte); // applique la couleur du texte
        LabelIcone.TextColor = Color.FromArgb(icone); // applique la couleur de l'icône
    }

    private void OnTape(object? sender, TappedEventArgs e)
    {
        if (IsEnabled) Clicked?.Invoke(this, EventArgs.Empty); // prévient la page si actif
    }
}