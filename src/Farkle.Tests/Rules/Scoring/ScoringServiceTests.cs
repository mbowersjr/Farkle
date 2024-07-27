using Farkle.Rules.Scoring;
using Farkle.Rules.DiceTypes;

namespace Farkle.Tests.Rules.Scoring;

public class ScoringServiceTests
{
    [Theory]
    // One
    [InlineData(100, 1)]
    // Five
    [InlineData(50, 5)]
    // Three of a Kind
    [InlineData(1000, 1, 1, 1)]
    [InlineData(200, 2, 2, 2)]
    [InlineData(300, 3, 3, 3)]
    [InlineData(400, 4, 4, 4)]
    [InlineData(500, 5, 5, 5)]
    [InlineData(600, 6, 6, 6)]
    // Four of a Kind
    [InlineData(1000, 1, 1, 1, 1)]
    [InlineData(1000, 2, 2, 2, 2)]
    [InlineData(1000, 3, 3, 3, 3)]
    [InlineData(1000, 4, 4, 4, 4)]
    [InlineData(1000, 5, 5, 5, 5)]
    [InlineData(1000, 6, 6, 6, 6)]
    // Five of a Kind
    [InlineData(2000, 1, 1, 1, 1, 1)]
    [InlineData(2000, 2, 2, 2, 2, 2)]
    [InlineData(2000, 3, 3, 3, 3, 3)]
    [InlineData(2000, 4, 4, 4, 4, 4)]
    [InlineData(2000, 5, 5, 5, 5, 5)]
    [InlineData(2000, 6, 6, 6, 6, 6)]
    // Six of a Kind
    [InlineData(3000, 1, 1, 1, 1, 1, 1)]
    [InlineData(3000, 2, 2, 2, 2, 2, 2)]
    [InlineData(3000, 3, 3, 3, 3, 3, 3)]
    [InlineData(3000, 4, 4, 4, 4, 4, 4)]
    [InlineData(3000, 5, 5, 5, 5, 5, 5)]
    [InlineData(3000, 6, 6, 6, 6, 6, 6)]
    // Straight 1-6
    [InlineData(2500, 1, 2, 3, 4, 5, 6)]
    public void CalculateScore_StandardCombinations_ReturnsCorrectScore(int expectedScore, params int[] diceValues)
    {
        List<DiceBase> dice = new List<DiceBase>();
        for (int i = 0; i < diceValues.Length; i++)
        {
            dice.Add(new StandardDice() { Value = diceValues[i] });
        }

        ScoringService scoringService = new ScoringService(allowNonStandardCombinations: false);

        ScoredSet scoredSet = scoringService.CalculateScore(dice);

        Assert.Equal(expectedScore, scoredSet.Score);
    }

    [Theory]
    // Two Triplets (Non-standard Rules)
    [InlineData(2500, true, 1, 1, 1, 2, 2, 2)]
    [InlineData(2500, true, 1, 1, 1, 3, 3, 3)]
    [InlineData(2500, true, 4, 4, 4, 5, 5, 5)]
    [InlineData(2500, true, 4, 4, 4, 6, 6, 6)]
    [InlineData(0, false, 1, 1, 1, 2, 2, 2)]
    [InlineData(0, false, 1, 1, 1, 3, 3, 3)]
    [InlineData(0, false, 4, 4, 4, 5, 5, 5)]
    [InlineData(0, false, 4, 4, 4, 6, 6, 6)]
    // Four of a Kind and a Pair (Non-standard Rules)
    [InlineData(1500, true, 1, 1, 1, 1, 2, 2)]
    [InlineData(1500, true, 1, 1, 1, 1, 3, 3)]
    [InlineData(1500, true, 4, 4, 4, 4, 5, 5)]
    [InlineData(1500, true, 4, 4, 4, 4, 6, 6)]
    [InlineData(0, false, 1, 1, 1, 1, 2, 2)]
    [InlineData(0, false, 1, 1, 1, 1, 3, 3)]
    [InlineData(0, false, 4, 4, 4, 4, 5, 5)]
    [InlineData(0, false, 4, 4, 4, 4, 6, 6)]
    public void CalculateScore_NonStandardCombinations_ReturnsCorrectScore(int expectedScore, bool allowNonStandardCombinations, params int[] diceValues)
    {
        List<DiceBase> dice = new List<DiceBase>();
        for (int i = 0; i < diceValues.Length; i++)
        {
            dice.Add(new StandardDice() { Value = diceValues[i] });
        }

        ScoringService scoringService = new ScoringService(allowNonStandardCombinations);

        ScoredSet scoredSet = scoringService.CalculateScore(dice);

        Assert.Equal(expectedScore, scoredSet.Score);
    }
}