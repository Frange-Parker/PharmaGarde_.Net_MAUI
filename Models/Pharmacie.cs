using System.Globalization;
using PharmaGarde.Helpers;

namespace PharmaGarde.Models;

public class Pharmacie
{
    // ---------- données de la pharmacie ----------
    public int Id { get; set; }                       // identifiant
    public string Nom { get; set; } = "";             // nom affiché
    public string Quartier { get; set; } = "";       // quartier (pour le filtre)
    public string Adresse { get; set; } = "";         // adresse complète
    public string Repere { get; set; } = "";          // repère ("Face Ancien Cinéma...")
    public string Telephone { get; set; } = "";       // numéro à appeler
    public double Latitude { get; set; }              // coordonnées GPS
    public double Longitude { get; set; }
    public double DistanceKm { get; set; }            // distance en km
    public bool OuvertMaintenant { get; set; }        // ouverte en ce moment ?
    public bool Permanence24h { get; set; }           // service continu 24h/24 ?
    public bool Tdr { get; set; }                     // service : test paludisme
    public bool MobileMoney { get; set; }             // service : Mobile Money
    public bool Oxygene { get; set; }                 // service : oxygénothérapie
    public string HorairesJour { get; set; } = "";    // horaires d'ouverture
    public string LibelleGarde { get; set; } = "Permanence de Garde";
    public string HorairesGarde { get; set; } = "";   // horaires de garde

    // ---------- textes de la carte de la liste ----------
    public string Rubrique { get; set; } = "";        // petit titre au-dessus du nom
    public string IconeDistance { get; set; } = Icons.Walk; // icône à côté de la distance
    public string InfoHoraire { get; set; } = "";     // texte à droite de la distance
    public string IconeInfo { get; set; } = "";       // icône de ce texte (vide = aucune)
    public bool InfoHoraireVert { get; set; }         // texte en vert ?
    public string Note { get; set; } = "";            // ligne d'information du bas
    public string IconeNote { get; set; } = Icons.Info;
    public bool AvecBadge { get; set; } = true;       // afficher le badge "Ouvert de garde" ?

    // ---------- état posé par la page au moment de l'affichage ----------
    public bool EstPremiere { get; set; }             // première carte = la plus proche
    public bool ItineraireActif { get; set; } = true; // faux en mode hors ligne

    // ---------- valeurs calculées pour l'affichage ----------
    public bool EstAutre => !EstPremiere;
    public string RubriqueAffichee => EstPremiere ? "LA PLUS PROCHE" : Rubrique;
    public Color CouleurRubrique => Color.FromArgb(EstPremiere ? "#3DDC6F" : "#B0B0B0");
    public bool AffBadge => AvecBadge || !OuvertMaintenant; // une pharmacie fermée affiche toujours son statut
    public string BadgeTexte => OuvertMaintenant ? "● Ouvert de garde" : "● Fermé";
    public Color BadgeFond => Color.FromArgb(OuvertMaintenant ? "#1B3A26" : "#3B1414");
    public Color BadgeCouleur => Color.FromArgb(OuvertMaintenant ? "#3DDC6F" : "#FF8A80");
    public bool AIconeInfo => !string.IsNullOrEmpty(IconeInfo);
    public Color CouleurInfo => Color.FromArgb(InfoHoraireVert ? "#3DDC6F" : "#B0B0B0");

    // "650" + "m"  ou  "1,4" + "km"
    public string DistanceValeur => DistanceKm < 1
        ? (DistanceKm * 1000).ToString("0")
        : DistanceKm.ToString("0.#", CultureInfo.GetCultureInfo("fr-FR"));
    public string DistanceUnite => DistanceKm < 1 ? "m" : "km";
}