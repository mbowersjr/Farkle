using Farkle.Rules.Scoring;
using Farkle.Rules.DiceTypes;

namespace Farkle.Tests.Rules.Scoring;

public class ScoringServiceTests
{
    [Theory]
    // One
    [InlineData(100, ScoredCombination.One, 1)]
    // Five
    [InlineData(50, ScoredCombination.Five, 5)]
    // Three of a Kind
    [InlineData(1000, ScoredCombination.ThreeOfAKind, 1, 1, 1)]
    [InlineData(200, ScoredCombination.ThreeOfAKind, 2, 2, 2)]
    [InlineData(300, ScoredCombination.ThreeOfAKind, 3, 3, 3)]
    [InlineData(400, ScoredCombination.ThreeOfAKind, 4, 4, 4)]
    [InlineData(500, ScoredCombination.ThreeOfAKind, 5, 5, 5)]
    [InlineData(600, ScoredCombination.ThreeOfAKind, 6, 6, 6)]
    // Four of a Kind
    [InlineData(2000, ScoredCombination.FourOfAKind, 1, 1, 1, 1)]
    [InlineData(400, ScoredCombination.FourOfAKind, 2, 2, 2, 2)]
    [InlineData(600, ScoredCombination.FourOfAKind, 3, 3, 3, 3)]
    [InlineData(800, ScoredCombination.FourOfAKind, 4, 4, 4, 4)]
    [InlineData(1000, ScoredCombination.FourOfAKind, 5, 5, 5, 5)]
    [InlineData(1200, ScoredCombination.FourOfAKind, 6, 6, 6, 6)]
    // Five of a Kind
    [InlineData(4000, ScoredCombination.FiveOfAKind, 1, 1, 1, 1, 1)]
    [InlineData(800, ScoredCombination.FiveOfAKind, 2, 2, 2, 2, 2)]
    [InlineData(1200, ScoredCombination.FiveOfAKind, 3, 3, 3, 3, 3)]
    [InlineData(1600, ScoredCombination.FiveOfAKind, 4, 4, 4, 4, 4)]
    [InlineData(2000, ScoredCombination.FiveOfAKind, 5, 5, 5, 5, 5)]
    [InlineData(2400, ScoredCombination.FiveOfAKind, 6, 6, 6, 6, 6)]
    // Six of a Kind
    [InlineData(8000, ScoredCombination.SixOfAKind, 1, 1, 1, 1, 1, 1)]
    [InlineData(1600, ScoredCombination.SixOfAKind, 2, 2, 2, 2, 2, 2)]
    [InlineData(2400, ScoredCombination.SixOfAKind, 3, 3, 3, 3, 3, 3)]
    [InlineData(3200, ScoredCombination.SixOfAKind, 4, 4, 4, 4, 4, 4)]
    [InlineData(4000, ScoredCombination.SixOfAKind, 5, 5, 5, 5, 5, 5)]
    [InlineData(4800, ScoredCombination.SixOfAKind, 6, 6, 6, 6, 6, 6)]
    // Straight 1-6
    [InlineData(1500, ScoredCombination.Straight, 1, 2, 3, 4, 5, 6)]
    // Partial Straight 1-5
    [InlineData(500, ScoredCombination.PartialStraightLow, 1, 2, 3, 4, 5)]
    // Partial Straight 2-6
    [InlineData(750, ScoredCombination.PartialStraightHigh, 2, 3, 4, 5, 6)]
    public void CalculateScore_ReturnsCorrectScoreAndCombination(int expectedScore, ScoredCombination expectedCombination, params int[] diceValues)
    {
        List<DiceBase> dice = new List<DiceBase>();
        for (int i = 0; i < diceValues.Length; i++)
        {
            dice.Add(new StandardDice() { Value = diceValues[i] });
        }

        ScoringService scoringService = new ScoringService();

        ScoredSet scoredSet = scoringService.CalculateScore(dice);

        Assert.Equal(expectedScore, scoredSet.Score);
        Assert.Equal(expectedCombination, scoredSet.Combination);
    }
}