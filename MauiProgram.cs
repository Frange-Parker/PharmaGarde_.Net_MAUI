using Microsoft.Maui.Handlers; // pour personnaliser les contrôles natifs

namespace PharmaGarde; // espace de noms du projet

public static class MauiProgram // classe de démarrage
{
    public static MauiApp CreateMauiApp() // construit l'application
    {
        var builder = MauiApp.CreateBuilder(); // crée le constructeur
        builder
            .UseMauiApp<App>() // classe racine
            .ConfigureFonts(fonts => // déclare les polices
            {
                fonts.AddFont("MaterialIcons-Regular.ttf", "Icons"); // police d'icônes, alias "Icons"
            });

        // Champs de saisie : retire le cadre natif Android et centre le texte verticalement
        EntryHandler.Mapper.AppendToMapping("SansCadre", (handler, vue) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;      // supprime le fond et le trait natifs
            handler.PlatformView.SetPadding(0, 0, 0, 0); // supprime le décalage interne
            var horizontal = handler.PlatformView.Gravity & Android.Views.GravityFlags.HorizontalGravityMask; // garde l'alignement horizontal
            handler.PlatformView.Gravity = horizontal | Android.Views.GravityFlags.CenterVertical; // centre verticalement
#endif
        });

        // Sélecteur de date : même traitement
        DatePickerHandler.Mapper.AppendToMapping("SansCadre", (handler, vue) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;      // supprime le cadre natif
            handler.PlatformView.SetPadding(0, 0, 0, 0); // supprime le décalage
#endif
        });

        // Sélecteur d'heure : même traitement
        TimePickerHandler.Mapper.AppendToMapping("SansCadre", (handler, vue) =>
        {
#if ANDROID
            handler.PlatformView.Background = null;      // supprime le cadre natif
            handler.PlatformView.SetPadding(0, 0, 0, 0); // supprime le décalage
#endif
        });

        return builder.Build(); // construit l'application
    }
}