namespace Farkle.Rules.Scoring;

public class ScoringRules
{
    public bool AllowNonStandardCombinations { get; set; }
    public bool SixOnesWins { get; set; }

    public static ScoringRules Standard { get; } = new ScoringRules() { AllowNonStandardCombinations = false, SixOnesWins = false };
    public static ScoringRules Alternate { get; } = new ScoringRules() { AllowNonStandardCombinations = true, SixOnesWins = true };
}