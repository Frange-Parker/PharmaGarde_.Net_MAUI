using Microsoft.Maui.Handlers; // nécessaire pour personnaliser les contrôles natifs

namespace PharmaGarde;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder(); // crée le constructeur d'application
        builder
            .UseMauiApp<App>() // classe racine
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("MaterialIcons-Regular.ttf", "Icons"); // police d'icônes
            });

        // Supprime le cadre natif Android des champs et centre le texte verticalement
        EntryHandler.Mapper.AppendToMapping("SansCadre", (handler, vue) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;      // retire le fond et le trait natifs
            handler.PlatformView.SetPadding(0, 0, 0, 0); // retire le décalage interne
            // garde l'alignement horizontal d'origine, force le centrage vertical
            var horizontal = handler.PlatformView.Gravity & Android.Views.GravityFlags.HorizontalGravityMask;
            handler.PlatformView.Gravity = horizontal | Android.Views.GravityFlags.CenterVertical;
#endif
        });

        return builder.Build(); // construit l'application
    }
}