using PharmaGarde.Models;
using PharmaGarde.Services;

namespace PharmaGarde.Helpers;

// Données FICTIVES de démonstration : couvrent tous les statuts (de garde, 24h, prochaine, ancienne, aucune donnée)
public static class DonneesDemo
{
    public static BaseDonnees Creer()
    {
        var maintenant = DateTime.UtcNow; // les permanences sont calculées par rapport à "maintenant"
        var b = new BaseDonnees();        // base vide

        // ----- 5 pharmacies -----
        b.Pharmacies.Add(new Pharmacie { Id = 1, Nom = "Pharmacie du Centre", Quartier = "Akwa",
            Adresse = "Boulevard de la Liberté, Akwa, Douala", Repere = "Face Ancien Cinéma Le Wouri",
            Latitude = 4.0533, Longitude = 9.7110, Telephone = "+237699000001",
            Tdr = true, MobileMoney = true, Oxygene = true, Perfusion = true, SerumAntitetanique = true, Source = "Démonstration" });
        b.Pharmacies.Add(new Pharmacie { Id = 2, Nom = "Pharmacie des Cocotiers", Quartier = "Bonanjo",
            Adresse = "Avenue Charles de Gaulle, Bonanjo, Douala", Repere = "Près de la Place du Gouvernement",
            Latitude = 4.0470, Longitude = 9.6990, Telephone = "+237699000002", HorairesOuverture = "24h/24",
            Tdr = true, MobileMoney = true, Perfusion = true, Source = "Démonstration" });
        b.Pharmacies.Add(new Pharmacie { Id = 3, Nom = "Pharmacie de la Liberté", Quartier = "Bonapriso",
            Adresse = "Rue Joss, Bonapriso, Douala", Repere = "Angle Rue Joss",
            Latitude = 4.0610, Longitude = 9.6960, Telephone = "+237699000003",
            Tdr = true, Source = "Démonstration" });
        b.Pharmacies.Add(new Pharmacie { Id = 4, Nom = "Pharmacie de Deïdo", Quartier = "Deïdo",
            Adresse = "Rue Manga Bell, Deïdo, Douala", Repere = "Près du marché de Deïdo",
            Latitude = 4.0690, Longitude = 9.7220, Telephone = "+237699000004",
            Tdr = true, MobileMoney = true, Oxygene = true, SerumAntitetanique = true, Source = "Démonstration" });
        b.Pharmacies.Add(new Pharmacie { Id = 5, Nom = "Pharmacie de Bépanda", Quartier = "Bépanda",
            Adresse = "Carrefour Bépanda, Douala", Repere = "Près du carrefour",
            Latitude = 4.0750, Longitude = 9.7330, Telephone = "+237699000005",
            Tdr = true, Source = "Démonstration" }); // non inscrite : aucune donnée

        // ----- comptes pharmaciens des 4 premières (mot de passe : Pharma2026) -----
        string[] emails = { "demo@pharmagarde.cm", "cocotiers@pharmagarde.cm", "liberte@pharmagarde.cm", "deido@pharmagarde.cm" };
        string[] noms = { "Dr E. Ngando", "Dr A. Tchoumi", "Dr M. Fotso", "Dr S. Mballa" };
        for (int i = 0; i < 4; i++)
        {
            string sel = Securite.NouveauSel(); // sel propre à chaque compte
            b.Pharmaciens.Add(new Pharmacien { Id = i + 1, PharmacieId = i + 1, NomComplet = noms[i], Email = emails[i],
                Sel = sel, MotDePasseHash = Securite.Hacher("Pharma2026", sel),
                NumeroOnpc = $"ONPC-DEMO-000{i + 1}", Statut = StatutCompte.Valide });
        }

        // ----- permanences -----
        // 1. Centre : garde de nuit en cours
        b.Permanences.Add(new Permanence { PharmacieId = 1, Type = TypePermanence.Nuit,
            DebutUtc = maintenant.AddHours(-2), FinUtc = maintenant.AddHours(10),
            NoteAcces = "Interphone extérieur & sonnette sécurisée de nuit", PharmacienDePermanence = "Dr E. Ngando",
            MisAJourLe = maintenant.AddHours(-3) });
        // 2. Cocotiers : service continu 24h/24
        b.Permanences.Add(new Permanence { PharmacieId = 2, Type = TypePermanence.Continu24h,
            DebutUtc = maintenant.AddHours(-24), FinUtc = maintenant.AddHours(72),
            NoteAcces = "Accès direct au guichet sans sonnette préalable", PharmacienDePermanence = "Dr A. Tchoumi",
            MisAJourLe = maintenant.AddHours(-26) });
        // 3. Liberté : une garde terminée il y a 12 jours (information ancienne)
        b.Permanences.Add(new Permanence { PharmacieId = 3, Type = TypePermanence.Nuit,
            DebutUtc = maintenant.AddDays(-12).AddHours(-12), FinUtc = maintenant.AddDays(-12),
            NoteAcces = "Sonnette à l'entrée", PharmacienDePermanence = "Dr M. Fotso",
            MisAJourLe = maintenant.AddDays(-13) });
        // 4. Deïdo : garde de week-end à venir (prochaine garde)
        b.Permanences.Add(new Permanence { PharmacieId = 4, Type = TypePermanence.WeekEnd,
            DebutUtc = maintenant.AddDays(2), FinUtc = maintenant.AddDays(4),
            NoteAcces = "Entrée de plain-pied & stationnement d'urgence réservé", PharmacienDePermanence = "Dr S. Mballa",
            MisAJourLe = maintenant.AddHours(-5) });
        // 5. Bépanda : volontairement aucune permanence

        return b; // base prête
    }
}