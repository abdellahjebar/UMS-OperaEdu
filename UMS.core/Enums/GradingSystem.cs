namespace UMS.Core.Enums
{
    /// <summary>
    /// Grading systems supported by the institution
    /// </summary>
    public enum GradingSystem
    {
        French = 1,           // 0-20 scale (Morocco, France, Tunisia)
        American = 2,         // 4.0 GPA scale
        Percentage = 3,       // 0-100% scale
        PassFail = 4,         // Simple Pass/Fail
        LetterGrade = 5       // A, B, C, D, F
    }
}
