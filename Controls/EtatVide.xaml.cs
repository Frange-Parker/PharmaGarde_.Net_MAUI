using PharmaGarde.Helpers;

namespace PharmaGarde.Controls;

public partial class EtatVide : ContentView
{
    public static readonly BindableProperty IconeProperty = BindableProperty.Create(nameof(Icone), typeof(string), typeof(EtatVide), Icons.Info,
        propertyChanged: (b, o, n) => ((EtatVide)b).LblIcone.Text = (string)n);
    public static readonly BindableProperty TitreProperty = BindableProperty.Create(nameof(Titre), typeof(string), typeof(EtatVide), "",
        propertyChanged: (b, o, n) => ((EtatVide)b).LblTitre.Text = (string)n);
    public static readonly BindableProperty MessageProperty = BindableProperty.Create(nameof(Message), typeof(string), typeof(EtatVide), "",
        propertyChanged: (b, o, n) => ((EtatVide)b).LblMessage.Text = (string)n);
    public static readonly BindableProperty TexteBoutonProperty = BindableProperty.Create(nameof(TexteBouton), typeof(string), typeof(EtatVide), "",
        propertyChanged: (b, o, n) => { var c = (EtatVide)b; c.Bouton.Text = (string)n; c.Bouton.IsVisible = !string.IsNullOrEmpty((string)n); });
    public static readonly BindableProperty IconeBoutonProperty = BindableProperty.Create(nameof(IconeBouton), typeof(string), typeof(EtatVide), "",
        propertyChanged: (b, o, n) => ((EtatVide)b).Bouton.Icon = (string)n);
    public static readonly BindableProperty VarianteBoutonProperty = BindableProperty.Create(nameof(VarianteBouton), typeof(string), typeof(EtatVide), "Primary",
        propertyChanged: (b, o, n) => ((EtatVide)b).Bouton.Variant = (string)n);
    public static readonly BindableProperty TexteLienProperty = BindableProperty.Create(nameof(TexteLien), typeof(string), typeof(EtatVide), "",
        propertyChanged: (b, o, n) => { var c = (EtatVide)b; c.LblLien.Text = (string)n; c.LblLien.IsVisible = !string.IsNullOrEmpty((string)n); });

    public string Icone { get => (string)GetValue(IconeProperty); set => SetValue(IconeProperty, value); }
    public string Titre { get => (string)GetValue(TitreProperty); set => SetValue(TitreProperty, value); }
    public string Message { get => (string)GetValue(MessageProperty); set => SetValue(MessageProperty, value); }
    public string TexteBouton { get => (string)GetValue(TexteBoutonProperty); set => SetValue(TexteBoutonProperty, value); }
    public string IconeBouton { get => (string)GetValue(IconeBoutonProperty); set => SetValue(IconeBoutonProperty, value); }
    public string VarianteBouton { get => (string)GetValue(VarianteBoutonProperty); set => SetValue(VarianteBoutonProperty, value); }
    public string TexteLien { get => (string)GetValue(TexteLienProperty); set => SetValue(TexteLienProperty, value); }

    public event EventHandler? BoutonClique; // le bouton a été touché
    public event EventHandler? LienClique;   // le lien a été touché

    public EtatVide()
    {
        InitializeComponent(); // charge le XAML
        LblIcone.Text = Icone; // icône par défaut
    }

    private void OnBouton(object? sender, EventArgs e) => BoutonClique?.Invoke(this, EventArgs.Empty);
    private void OnLien(object? sender, TappedEventArgs e) => LienClique?.Invoke(this, EventArgs.Empty);
}