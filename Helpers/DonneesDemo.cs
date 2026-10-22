using PharmaGarde.Models;

namespace PharmaGarde.Helpers;

// Données fictives de démonstration (à remplacer plus tard par l'API REST)
public static class DonneesDemo
{
    public static readonly List<Pharmacie> Pharmacies = new()
    {
        new Pharmacie
        {
            Id = 1, Nom = "Pharmacie du Centre", Quartier = "Akwa",
            Adresse = "Boulevard de la Liberté, Akwa, Douala", Repere = "Face Ancien Cinéma Le Wouri",
            Telephone = "+237699000001", Latitude = 4.0511, Longitude = 9.7085, DistanceKm = 0.65,
            OuvertMaintenant = true, Permanence24h = false, Tdr = true, MobileMoney = true, Oxygene = true,
            HorairesJour = "08h30 - 20h00", HorairesGarde = "20h00 - 08h30",
            Rubrique = "LA PLUS PROCHE", InfoHoraire = "~8 min de marche", IconeInfo = Icons.Clock,
            Note = "Garde de nuit : 20h00 - 08h00", IconeNote = Icons.Home
        },
        new Pharmacie
        {
            Id = 2, Nom = "Pharmacie des Cocotiers", Quartier = "Bonanjo",
            Adresse = "Avenue Charles de Gaulle, Bonanjo, Douala", Repere = "Près du Rond-point Deido",
            Telephone = "+237699000002", Latitude = 4.0470, Longitude = 9.6930, DistanceKm = 1.4,
            OuvertMaintenant = true, Permanence24h = true, Tdr = true, MobileMoney = true, Oxygene = false,
            HorairesJour = "24h/24", HorairesGarde = "Service continu",
            Rubrique = "SERVICE CONTINU", InfoHoraire = "Permanence 24h/24", InfoHoraireVert = true,
            Note = "Accès direct au guichet sans sonnette préalable", IconeNote = Icons.Info, AvecBadge = false
        },
        new Pharmacie
        {
            Id = 3, Nom = "Pharmacie de la Liberté", Quartier = "Akwa",
            Adresse = "Rue Joss, Akwa, Douala", Repere = "Angle Rue Joss",
            Telephone = "+237699000003", Latitude = 4.0560, Longitude = 9.7020, DistanceKm = 1.9,
            OuvertMaintenant = false, Permanence24h = false, Tdr = true, MobileMoney = false, Oxygene = false,
            HorairesJour = "08h30 - 20h00", HorairesGarde = "Pas de garde cette nuit",
            Rubrique = "OFFICINE DE QUARTIER", InfoHoraire = "Ouverte de 08h30 à 20h00", IconeInfo = Icons.Clock,
            Note = "Fermée la nuit : voir les pharmacies de garde", IconeNote = Icons.Info
        },
        new Pharmacie
        {
            Id = 4, Nom = "Pharmacie de Deïdo", Quartier = "Deïdo",
            Adresse = "Rue Manga Bell, Deido, Douala", Repere = "Près du marché Deido",
            Telephone = "+237699000004", Latitude = 4.0650, Longitude = 9.7160, DistanceKm = 2.8,
            OuvertMaintenant = true, Permanence24h = false, Tdr = true, MobileMoney = true, Oxygene = true,
            HorairesJour = "08h30 - 20h00", HorairesGarde = "Week-end & jours fériés",
            Rubrique = "GRAND PÔLE MÉDICAL", IconeDistance = Icons.Car,
            InfoHoraire = "Garde week-end & jour férié", IconeInfo = Icons.Calendar,
            Note = "Entrée de plain-pied & stationnement d'urgence réservé", IconeNote = Icons.Accessible
        }
    };

    // retrouve une pharmacie par son identifiant (la première par défaut)
    public static Pharmacie Trouver(int id) => Pharmacies.FirstOrDefault(p => p.Id == id) ?? Pharmacies[0];
}