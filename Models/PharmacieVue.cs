using System.Globalization;
using PharmaGarde.Helpers;

namespace PharmaGarde.Models;

public class PharmacieVue
{
    private static readonly CultureInfo Fr = CultureInfo.GetCultureInfo("fr-FR"); // formats français

    // ---------- données calculées par le service ----------
    public Pharmacie Pharmacie { get; set; } = new();                   // données brutes
    public StatutGarde Statut { get; set; } = StatutGarde.AucuneDonnee; // statut (RG2)
    public Permanence? Permanence { get; set; }                         // permanence retenue
    public bool Ancienne { get; set; }                                  // information plus vieille que le seuil
    public bool EstInscrite { get; set; }                               // pharmacien validé ?
    public double DistanceKm { get; set; }                              // distance depuis l'origine
    public bool EstPremiere { get; set; }                               // première carte de la liste
    public bool ItineraireActif { get; set; } = true;                   // faux en mode hors ligne

    // ---------- raccourcis ----------
    public int Id => Pharmacie.Id;
    public string Nom => Pharmacie.Nom;
    public string Adresse => Pharmacie.Adresse;
    public string Repere => Pharmacie.Repere;
    public string Quartier => Pharmacie.Quartier;
    public string Telephone => Pharmacie.Telephone;
    public double Latitude => Pharmacie.Latitude;
    public double Longitude => Pharmacie.Longitude;
    public bool EstDeGarde => Statut == StatutGarde.DeGarde;           // de garde en ce moment ?
    public bool AvecDonnees => Statut != StatutGarde.AucuneDonnee;     // a-t-on une information ?
    public bool EstAutre => !EstPremiere;                              // carte "normale"

    // ---------- distance et durées ----------
    public string DistanceValeur => DistanceKm < 1
        ? Math.Round(DistanceKm * 1000).ToString("0", Fr)              // "650"
        : DistanceKm.ToString("0.#", Fr);                              // "1,4"
    public string DistanceUnite => DistanceKm < 1 ? "m" : "km";        // unité
    public string DistanceTexte => DistanceValeur + " " + DistanceUnite; // "650 m"
    public int MinutesPied => Math.Max(1, (int)Math.Round(DistanceKm / 5.0 * 60));     // marche à 5 km/h
    public int MinutesVoiture => Math.Max(1, (int)Math.Round(DistanceKm / 25.0 * 60)); // voiture à 25 km/h en ville
    public string TempsPied => $"~{MinutesPied} min à pied";           // "~8 min à pied"
    public string TempsPiedCourt => $"{MinutesPied} min";              // "8 min"
    public string TempsVoiture => $"{MinutesVoiture} min";             // "3 min"
    public string IconeDistance => DistanceKm <= 1.5 ? Icons.Walk : Icons.Car; // marche ou voiture

    // ---------- textes et couleurs selon le statut ----------
    public string Rubrique
    {
        get
        {
            if (Statut == StatutGarde.DeGarde)
            {
                if (EstPremiere) return "LA PLUS PROCHE"; // première de la liste
                return Permanence?.Type switch
                {
                    TypePermanence.Continu24h => "SERVICE CONTINU",
                    TypePermanence.WeekEnd => "GARDE WEEK-END",
                    TypePermanence.JourFerie => "GARDE JOUR FÉRIÉ",
                    _ => "GARDE DE NUIT"
                };
            }
            if (Statut == StatutGarde.ProchaineGarde) return "PROCHAINE GARDE";
            if (Statut == StatutGarde.InformationAncienne) return "INFORMATION ANCIENNE";
            return "AUCUNE DONNÉE";
        }
    }

    // vert pour "la plus proche" en garde, gris sinon
    public Color CouleurRubrique => Color.FromArgb(EstDeGarde && EstPremiere ? "#3DDC6F" : "#B0B0B0");

    public string BadgeTexte => Statut switch
    {
        StatutGarde.DeGarde => "● Ouvert de garde",
        StatutGarde.ProchaineGarde => "● Prochaine garde",
        StatutGarde.InformationAncienne => "● Information ancienne",
        _ => "● Aucune donnée"
    };

    public Color BadgeFond => Color.FromArgb(Statut switch
    {
        StatutGarde.DeGarde => "#1B3A26",
        StatutGarde.ProchaineGarde => "#14222E",
        StatutGarde.InformationAncienne => "#3D2F10",
        _ => "#2A2A2A"
    });

    public Color BadgeCouleur => Color.FromArgb(Statut switch
    {
        StatutGarde.DeGarde => "#3DDC6F",
        StatutGarde.ProchaineGarde => "#7CC4FF",
        StatutGarde.InformationAncienne => "#FFB74D",
        _ => "#B0B0B0"
    });

    // phrase d'information horaire
    public string InfoHoraire
    {
        get
        {
            var p = Permanence; // permanence retenue
            if (p == null) return "Aucune permanence enregistrée";
            return Statut switch
            {
                StatutGarde.DeGarde => p.Type == TypePermanence.Continu24h
                    ? "Permanence 24h/24"
                    : $"Garde jusqu'à {Heure.Court(p.FinUtc)}",
                StatutGarde.ProchaineGarde => $"Garde le {Heure.Jour(p.DebutUtc)} à {Heure.Court(p.DebutUtc)}",
                StatutGarde.InformationAncienne => $"Dernière garde le {Heure.Jour(p.FinUtc)}",
                _ => "Aucune permanence enregistrée"
            };
        }
    }

    // icône de la phrase d'information
    public string IconeInfo => Statut == StatutGarde.InformationAncienne ? Icons.History
        : AvecDonnees ? Icons.Clock : Icons.Info;

    // texte vert si de garde, gris sinon
    public Color CouleurInfo => Color.FromArgb(EstDeGarde ? "#3DDC6F" : "#B0B0B0");

    // ligne de note en bas de carte
    public string Note
    {
        get
        {
            if (Permanence != null && !string.IsNullOrWhiteSpace(Permanence.NoteAcces)) return Permanence.NoteAcces; // note du pharmacien
            return AvecDonnees
                ? "Appel de confirmation recommandé avant tout déplacement nocturne"
                : "Aucune donnée sur cette pharmacie";
        }
    }
    public string IconeNote => Icons.Info; // icône de la note

    // "Mis à jour le 23/10/2026 à 14:30"
    public string MajTexte => Permanence != null
        ? $"Mis à jour le {Heure.DateHeure(Permanence.MisAJourLe)}"
        : "Aucune donnée sur cette pharmacie";

    // "23/10 20h00 - 24/10 08h00"
    public string PlageGarde => Permanence == null
        ? "Aucune permanence"
        : $"{Heure.JourHeure(Permanence.DebutUtc)} - {Heure.JourHeure(Permanence.FinUtc)}";
}