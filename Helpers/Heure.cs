using System.Globalization;

namespace PharmaGarde.Helpers;

public static class Heure
{
    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr-FR"); // formats français
    public static readonly TimeSpan Decalage = TimeSpan.FromHours(1); // Cameroun = UTC+1, sans heure d'été

    // UTC -> heure locale du Cameroun
    public static DateTime VersLocal(DateTime utc) => DateTime.SpecifyKind(utc, DateTimeKind.Utc).Add(Decalage);

    // heure locale du Cameroun -> UTC
    public static DateTime VersUtc(DateTime local) => DateTime.SpecifyKind(local.Subtract(Decalage), DateTimeKind.Utc);

    // "23/10/2026 à 14:30"
    public static string DateHeure(DateTime utc) => VersLocal(utc).ToString("dd/MM/yyyy 'à' HH:mm", Fr);

    // "14h30"
    public static string Court(DateTime utc) => VersLocal(utc).ToString("HH'h'mm", Fr);

    // "23/10"
    public static string Jour(DateTime utc) => VersLocal(utc).ToString("dd/MM", Fr);

    // "23/10 14h30"
    public static string JourHeure(DateTime utc) => VersLocal(utc).ToString("dd/MM HH'h'mm", Fr);
}