namespace UMS.Core.Enums
{
    /// <summary>
    /// Grade types with French/Moroccan equivalents (0-20 scale)
    /// </summary>
    public enum GradeType
    {
        // Excellent grades (18-20/20 = Très Bien = 4.0 GPA)
        APlus = 2,        // 18-20/20
        
        // Very Good grades (16-18/20 = Très Bien = 3.7-3.9 GPA)
        A = 1,            // 16-18/20
        
        // Good grades (14-16/20 = Bien = 3.3-3.6 GPA)
        AMinus = 3,       // 14-16/20
        BPlus = 4,        // 13-14/20
        
        // Above Average grades (12-13/20 = Assez Bien = 2.7-3.2 GPA)
        B = 5,            // 12-13/20
        BMinus = 6,       // 11-12/20
        
        // Average grades (10-11/20 = Passable = 2.0-2.6 GPA)
        CPlus = 7,        // 10-11/20
        C = 8,            // 10/20
        
        // Below Average grades (8-10/20 = Insuffisant = 1.3-1.9 GPA)
        CMinus = 9,       // 8-10/20
        DPlus = 10,       // 7-8/20
        
        // Poor grades (5-7/20 = Médiocre = 1.0-1.2 GPA)
        D = 11,           // 5-7/20
        
        // Failing grade (0-5/20 = Très Insuffisant = 0.0 GPA)
        F = 12,           // 0-5/20
        
        // Special statuses
        Incomplete = 13,  // Course not yet completed
        Withdrawn = 14,   // Student withdrew from course
        Pass = 15,        // Pass/Fail grading - Passed
        Fail = 16         // Pass/Fail grading - Failed
    }
}
