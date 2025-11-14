namespace UMS.Core.Enums
{
    public enum DegreeType
    {
        // ============================================
        // SYSTÈME INTERNATIONAL (Anglophone)
        // ============================================
        Certificate = 1,
        Associate = 2,
        Bachelor = 3,
        Master = 4,
        Doctorate = 5,

        // ============================================
        // SYSTÈME LMD - MAROC/FRANCE (Bac+3/5/8)
        // ============================================
        Licence = 10,                    // Bac+3 (Licence fondamentale/professionnelle)
        LicenceProfessionnelle = 11,     // Bac+3 (LP - orientation professionnelle)
        Master1 = 12,                    // Bac+4 (M1)
        Master2 = 13,                    // Bac+5 (M2 - Master spécialisé)
        MasterSpecialise = 14,           // Bac+5 (Master spécialisé)
        Doctorat = 15,                   // Bac+8 (PhD)

        // ============================================
        // DIPLÔMES BAC+2 - MAROC
        // ============================================
        DEUG = 20,                       // Diplôme d'Études Universitaires Générales (ancien système)
        DEUST = 21,                      // Diplôme d'Études Universitaires en Sciences et Techniques (Bac+2)
        DEUP = 22,                       // Diplôme d'Études Universitaires Professionnelles (Bac+2)
        DUT = 23,                        // Diplôme Universitaire de Technologie (Bac+2)
        BTS = 24,                        // Brevet de Technicien Supérieur (Bac+2)

        // ============================================
        // DIPLÔMES D'INGÉNIEUR - MAROC
        // ============================================
        IngenieurDiplome = 30,           // Diplôme d'Ingénieur d'État (Bac+5)
        IngenieurCycle = 31,             // Cycle Ingénieur (Bac+5 - grandes écoles)

        // ============================================
        // DIPLÔMES SPÉCIALISÉS - MAROC
        // ============================================
        DiplomeNational = 40,            // Diplômes nationaux divers
        DES = 41,                        // Diplôme d'Études Supérieures (ancien)
        DESA = 42,                       // Diplôme d'Études Supérieures Approfondies (ancien)
        DESS = 43,                       // Diplôme d'Études Supérieures Spécialisées (ancien)

        // ============================================
        // AUTRES
        // ============================================
        Diploma = 50,                    // Diplôme générique
        PreparationClasses = 51          // Classes Préparatoires (CPGE)
    }
}