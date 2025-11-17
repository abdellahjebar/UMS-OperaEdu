using UMS.Core.Enums;

namespace UMS.Core.ValueObjects
{
    /// <summary>
    /// Value object for grade conversions between different grading systems
    /// </summary>
    public class GradeScale
    {
        /// <summary>
        /// Convert French 0-20 scale to American 4.0 GPA
        /// </summary>
        public static decimal ConvertFrenchToGPA(decimal frenchGrade)
        {
            if (frenchGrade < 0 || frenchGrade > 20)
                throw new ArgumentException("French grade must be between 0 and 20", nameof(frenchGrade));

            return frenchGrade switch
            {
                >= 18 => 4.0m,          // Très Bien (Excellent)
                >= 16 => 3.7m + ((frenchGrade - 16) / 2 * 0.3m), // Très Bien
                >= 14 => 3.3m + ((frenchGrade - 14) / 2 * 0.4m), // Bien
                >= 12 => 2.7m + ((frenchGrade - 12) / 2 * 0.6m), // Assez Bien
                >= 10 => 2.0m + ((frenchGrade - 10) / 2 * 0.7m), // Passable
                >= 8 => 1.3m + ((frenchGrade - 8) / 2 * 0.7m),   // Insuffisant
                >= 5 => 1.0m + ((frenchGrade - 5) / 3 * 0.3m),   // Médiocre
                _ => 0.0m                                          // Très Insuffisant
            };
        }

        /// <summary>
        /// Convert French 0-20 scale to letter grade
        /// </summary>
        public static GradeType ConvertFrenchToLetterGrade(decimal frenchGrade)
        {
            if (frenchGrade < 0 || frenchGrade > 20)
                throw new ArgumentException("French grade must be between 0 and 20", nameof(frenchGrade));

            return frenchGrade switch
            {
                >= 18 => GradeType.APlus,
                >= 16 => GradeType.A,
                >= 14 => GradeType.AMinus,
                >= 13 => GradeType.BPlus,
                >= 12 => GradeType.B,
                >= 11 => GradeType.BMinus,
                >= 10 => GradeType.CPlus,
                >= 8 => GradeType.CMinus,
                >= 7 => GradeType.DPlus,
                >= 5 => GradeType.D,
                _ => GradeType.F
            };
        }

        /// <summary>
        /// Get French mention (appreciation) based on 0-20 grade
        /// </summary>
        public static string GetFrenchMention(decimal frenchGrade)
        {
            if (frenchGrade < 0 || frenchGrade > 20)
                throw new ArgumentException("French grade must be between 0 and 20", nameof(frenchGrade));

            return frenchGrade switch
            {
                >= 16 => "Très Bien",      // Very Good
                >= 14 => "Bien",            // Good
                >= 12 => "Assez Bien",      // Fairly Good
                >= 10 => "Passable",        // Pass
                >= 8 => "Insuffisant",      // Insufficient
                >= 5 => "Médiocre",         // Poor
                _ => "Très Insuffisant"     // Very Insufficient
            };
        }

        /// <summary>
        /// Convert American GPA to French 0-20 scale (approximate)
        /// </summary>
        public static decimal ConvertGPAToFrench(decimal gpa)
        {
            if (gpa < 0 || gpa > 4.0m)
                throw new ArgumentException("GPA must be between 0 and 4.0", nameof(gpa));

            return gpa switch
            {
                >= 3.9m => 18 + ((gpa - 3.9m) / 0.1m * 2),  // 18-20
                >= 3.7m => 16 + ((gpa - 3.7m) / 0.2m * 2),  // 16-18
                >= 3.3m => 14 + ((gpa - 3.3m) / 0.4m * 2),  // 14-16
                >= 2.7m => 12 + ((gpa - 2.7m) / 0.6m * 2),  // 12-14
                >= 2.0m => 10 + ((gpa - 2.0m) / 0.7m * 2),  // 10-12
                >= 1.3m => 8 + ((gpa - 1.3m) / 0.7m * 2),   // 8-10
                >= 1.0m => 5 + ((gpa - 1.0m) / 0.3m * 3),   // 5-8
                _ => gpa * 5                                 // 0-5
            };
        }

        /// <summary>
        /// Convert percentage (0-100) to French 0-20 scale
        /// </summary>
        public static decimal ConvertPercentageToFrench(decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Percentage must be between 0 and 100", nameof(percentage));

            return percentage / 5;  // Simple linear conversion
        }

        /// <summary>
        /// Convert percentage (0-100) to American GPA
        /// </summary>
        public static decimal ConvertPercentageToGPA(decimal percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentException("Percentage must be between 0 and 100", nameof(percentage));

            return percentage switch
            {
                >= 93 => 4.0m,
                >= 90 => 3.7m,
                >= 87 => 3.3m,
                >= 83 => 3.0m,
                >= 80 => 2.7m,
                >= 77 => 2.3m,
                >= 73 => 2.0m,
                >= 70 => 1.7m,
                >= 67 => 1.3m,
                >= 65 => 1.0m,
                _ => 0.0m
            };
        }
    }
}
