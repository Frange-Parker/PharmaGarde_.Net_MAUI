using System.Windows.Input;

namespace PharmaGarde.Controls;

public partial class IconButton : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(IconButton), string.Empty,
        propertyChanged: (b, o, n) => ((IconButton)b).Actualiser());
    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon), typeof(string), typeof(IconButton), string.Empty,
        propertyChanged: (b, o, n) => ((IconButton)b).Actualiser());
    public static readonly BindableProperty VariantProperty = BindableProperty.Create(
        nameof(Variant), typeof(string), typeof(IconButton), "Primary",
        propertyChanged: (b, o, n) => ((IconButton)b).Actualiser());
    // NOUVEAU : commande déclenchée au toucher (pour le MVVM)
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(
        nameof(Command), typeof(ICommand), typeof(IconButton), null);

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public string Variant { get => (string)GetValue(VariantProperty); set => SetValue(VariantProperty, value); }
    public ICommand? Command { get => (ICommand?)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

    public event EventHandler? Clicked; // événement déclenché au toucher (usage code-behind)

    public IconButton()
    {
        InitializeComponent(); // charge le XAML
        Actualiser();          // applique les couleurs initiales
    }

    // redessine le bouton quand IsEnabled change (état désactivé)
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(IsEnabled)) Actualiser();
    }

    // choisit les couleurs selon la variante et l'état
    private void Actualiser()
    {
        LabelTexte.Text = Text;
        LabelIcone.Text = Icon;
        LabelIcone.IsVisible = !string.IsNullOrEmpty(Icon);
        LabelTexte.IsVisible = !string.IsNullOrEmpty(Text);

        var (fond, texte, icone) = !IsEnabled
            ? ("#2A2A2A", "#6B6B6B", "#6B6B6B")                   // désactivé : gris
            : Variant switch
            {
                "Secondary" => ("#2A2A2A", "#EDEDED", "#3DDC6F"), // secondaire : gris, icône verte
                "Danger" => ("#3B1414", "#FF8A80", "#FF8A80"),    // danger : rouge sombre
                _ => ("#00C853", "#0B1F10", "#0B1F10")            // principal : vert
            };

        Boite.BackgroundColor = Color.FromArgb(fond);
        LabelTexte.TextColor = Color.FromArgb(texte);
        LabelIcone.TextColor = Color.FromArgb(icone);
    }

    private void OnTape(object? sender, TappedEventArgs e)
    {
        if (!IsEnabled) return; // bouton désactivé : rien
        Clicked?.Invoke(this, EventArgs.Empty); // événement (code-behind)
        if (Command != null && Command.CanExecute(null)) Command.Execute(null); // commande (MVVM)
    }
}