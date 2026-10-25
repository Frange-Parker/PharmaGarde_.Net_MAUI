using System.Security.Cryptography;

namespace PharmaGarde.Services;

public static class Securite
{
    // génère un sel aléatoire de 16 octets
    public static string NouveauSel() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));

    // mot de passe + sel -> empreinte (PBKDF2, SHA-256)
    public static string Hacher(string motDePasse, string sel)
    {
        byte[] octets = Rfc2898DeriveBytes.Pbkdf2(
            motDePasse, Convert.FromBase64String(sel), 10000, HashAlgorithmName.SHA256, 32); // 10 000 tours
        return Convert.ToBase64String(octets); // texte
    }

    // vérifie un mot de passe saisi contre l'empreinte enregistrée
    public static bool Verifier(string motDePasse, string sel, string empreinte)
    {
        byte[] a = Convert.FromBase64String(Hacher(motDePasse, sel)); // empreinte recalculée
        byte[] b = Convert.FromBase64String(empreinte);               // empreinte enregistrée
        return CryptographicOperations.FixedTimeEquals(a, b);         // comparaison à temps constant
    }
}