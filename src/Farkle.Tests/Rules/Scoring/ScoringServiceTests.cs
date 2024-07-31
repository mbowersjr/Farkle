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
    // Three Pair
    [InlineData(1500, 1, 1, 2, 2, 3, 3)]
    [InlineData(1500, 2, 2, 3, 3, 4, 4)]
    [InlineData(1500, 1, 1, 4, 4, 6, 6)]
    // Straight 1-6
    [InlineData(3000, 1, 2, 3, 4, 5, 6)]
    public void CalculateScore_StandardCombinations_ReturnsCorrectScore(int expectedScore, params int[] diceValues)
    {
        List<DiceBase> dice = new List<DiceBase>();
        for (int i = 0; i < diceValues.Length; i++)
        {
            dice.Add(new StandardDice() { Value = diceValues[i] });
        }

        ScoringService scoringService = new ScoringService(ScoringRules.Standard);

        ScoredSet scoredSet = scoringService.CalculateScore(dice);

        Assert.Equal(expectedScore, scoredSet.Score);
    }

    [Theory]
    // Two Triplets
    [InlineData(2500, true, 1, 1, 1, 2, 2, 2)]
    [InlineData(2500, true, 1, 1, 1, 3, 3, 3)]
    [InlineData(2500, true, 4, 4, 4, 5, 5, 5)]
    [InlineData(2500, true, 4, 4, 4, 6, 6, 6)]
    [InlineData(0, false, 4, 4, 4, 6, 6, 6)]
    // Four of a Kind
    [InlineData(2000, true, 1, 1, 1, 1)]
    [InlineData(400, true, 2, 2, 2, 2)]
    [InlineData(600, true, 3, 3, 3, 3)]
    [InlineData(800, true, 4, 4, 4, 4)]
    [InlineData(1000, true, 5, 5, 5, 5)]
    [InlineData(1200, true, 6, 6, 6, 6)]
    [InlineData(0, false, 6, 6, 6, 6)]
    // Five of a Kind
    [InlineData(3000, true, 1, 1, 1, 1, 1)]
    [InlineData(600, true, 2, 2, 2, 2, 2)]
    [InlineData(900, true, 3, 3, 3, 3, 3)]
    [InlineData(1200, true, 4, 4, 4, 4, 4)]
    [InlineData(1500, true, 5, 5, 5, 5, 5)]
    [InlineData(1800, true, 6, 6, 6, 6, 6)]
    [InlineData(0, false, 6, 6, 6, 6, 6)]
    // Six of a Kind
    [InlineData(10000, true, 1, 1, 1, 1, 1, 1)]
    [InlineData(10000, true, 2, 2, 2, 2, 2, 2)]
    [InlineData(10000, true, 3, 3, 3, 3, 3, 3)]
    [InlineData(10000, true, 4, 4, 4, 4, 4, 4)]
    [InlineData(10000, true, 5, 5, 5, 5, 5, 5)]
    [InlineData(10000, true, 6, 6, 6, 6, 6, 6)]
    [InlineData(0, false, 6, 6, 6, 6, 6, 6)]
    public void CalculateScore_NonStandardCombinations_ReturnsCorrectScore(int expectedScore, bool allowNonStandardCombinations, params int[] diceValues)
    {
        List<DiceBase> dice = new List<DiceBase>();
        for (int i = 0; i < diceValues.Length; i++)
        {
            dice.Add(new StandardDice() { Value = diceValues[i] });
        }

        ScoringService scoringService = new ScoringService(new ScoringRules() { AllowNonStandardCombinations = allowNonStandardCombinations });

        ScoredSet scoredSet = scoringService.CalculateScore(dice);

        Assert.Equal(expectedScore, scoredSet.Score);
    }
}