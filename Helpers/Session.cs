namespace PharmaGarde.Helpers;

// Mémoire de session : valable tant que l'application reste ouverte
public static class Session
{
    // vrai = mode invité (pas de compte), faux = utilisateur connecté
    public static bool EstInvite { get; set; } = true;
}