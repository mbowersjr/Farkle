using System;
using System.Linq;
using System.Collections.Generic;
using Farkle.Rules.DiceTypes;
using static System.Formats.Asn1.AsnWriter;

namespace Farkle.Rules.Scoring;

/// <summary>
/// Calculates a score given a set of selected dice.
/// </summary>
/// <remarks>
/// <para>Standard Rules</para>
/// <list type="table">
///     <item> <term> 1 </term> <description> 100 </description> </item>
///     <item> <term> 5 </term> <description> 50 </description> </item>
///     <item> <term> Three 1s </term> <description> 1000 </description> </item>
///     <item> <term> Three 2s </term> <description> 200 </description> </item>
///     <item> <term> Three 3s </term> <description> 300 </description> </item>
///     <item> <term> Three 4s </term> <description> 400 </description> </item>
///     <item> <term> Three 5s </term> <description> 500 </description> </item>
///     <item> <term> Three 6s </term> <description> 600 </description> </item>
///     <item> <term> Three Pairs </term> <description> 1500 </description> </item>
///     <item> <term> Straight (1-6) </term> <description> 3000 </description> </item>
/// </list>
/// 
/// <para>Non-Standard Rules</para>
/// <list type="table">
///     <item> <term> Two Triplets </term> <description> 2500 </description> </item>
///     <item> <term> Four of a Kind </term> <description> 2x Three of a Kind </description> </item>
///     <item> <term> Four of a Kind and a Pair </term> <description> 1500 </description> </item>
///     <item> <term> Five of a Kind </term> <description> 3x Three of a Kind score </description> </item>
///     <item> <term> Six of a Kind </term> <description> 10000 </description> </item>
///     <item> <term> Six 1s </term> <description> Wins the game if <see cref="ScoringRules.SixOnesWins"/> is true, otherwise 10000 </description> </item>
/// </list>
/// </remarks>
public class ScoringService
{
    public ScoringRules Rules { get; set; }

    public ScoringService(ScoringRules rules)
    {
        Rules = rules;
        InitializeCalculations();
    }

    private ScoredSet Calculate(IList<DiceBase> dice, ScoredCombination combination)
    {
        if (!_calculations.TryGetValue(combination, out var calculation))
            return ScoredSet.None;

        int[] diceValues = dice.GetValues();
        int score = calculation(diceValues);

        return new ScoredSet(dice, score, combination);
    }

    public ScoredSet CalculateScore(IList<DiceSprite> diceSprites)
    {
        var dice = diceSprites.Select(x => x.Dice).ToList();
        return CalculateScore(dice);
    }

    public ScoredSet CalculateScore(IList<DiceBase> dice)
    {
        ArgumentNullException.ThrowIfNull(dice);

        var values = dice.GetValues();
        var distinctValues = values.Distinct().ToList();

        if (dice.Count == 1)
        {
            if (dice[0].Value == 1)
            {
                // One 1
                return Calculate(dice, ScoredCombination.One);
            }
            
            if (dice[0].Value == 5)
            {
                // One 5
                return Calculate(dice, ScoredCombination.Five);
            }
        }
        else if (dice.Count == 3 && distinctValues.Count == 1)
        {
            // Three of a kind
            return Calculate(dice, ScoredCombination.ThreeOfAKind);
        }
        else if (dice.Count == 4 && distinctValues.Count == 1)
        {
            // Four of a kind
            return Calculate(dice, ScoredCombination.FourOfAKind);
        }
        else if (dice.Count == 5 && distinctValues.Count == 1)
        {
            // Five of a kind
            return Calculate(dice, ScoredCombination.FiveOfAKind);
        }
        else if (dice.Count == 6 && distinctValues.Count == 1)
        {
            // Six of a kind
            return Calculate(dice, ScoredCombination.SixOfAKind);
        }
        else if (dice.Count == 6 && distinctValues.Count == 2)
        {
            // Two triplets
            return Calculate(dice, ScoredCombination.TwoTriplets);
        }
        else if (dice.Count == 6 && distinctValues.Count == 3)
        {
            // Three pair
            return Calculate(dice, ScoredCombination.ThreePairs);
        }
        else if (dice.Count == 6 && distinctValues.Count == 6)
        {
            // Straight 1-6
            return Calculate(dice, ScoredCombination.Straight);
        }

        return ScoredSet.None;
    }

    public Dictionary<ScoredCombination, Func<int[], int>> _calculations = new Dictionary<ScoredCombination, Func<int[], int>>();

    private void InitializeCalculations()
    {
        _calculations.Add(ScoredCombination.One, values => 100);
        _calculations.Add(ScoredCombination.Five, values => 50);
        _calculations.Add(ScoredCombination.ThreeOfAKind, values =>
        {
            return values[0] switch
            {
                1 => 1000,
                _ => values[0] * 100
            };
        });
        _calculations.Add(ScoredCombination.FourOfAKind, values =>
        {
            if (!Rules.AllowNonStandardCombinations) return 0;
            return values[0] switch
            {
                1 => 2000,
                _ => values[0] * 100 * 2    // 2x Three of a kind score
            };
        });
        _calculations.Add(ScoredCombination.FiveOfAKind, values =>
        {
            if (!Rules.AllowNonStandardCombinations) return 0;
            return values[0] switch
            {
                1 => 3000,
                _ => values[0] * 100 * 3    // 3x Three of a kind score
            };
        });
        _calculations.Add(ScoredCombination.SixOfAKind, values =>
        {
            if (!Rules.AllowNonStandardCombinations) return 0;
            return 10000;
        });
        _calculations.Add(ScoredCombination.TwoTriplets, values =>
        {
            if (!Rules.AllowNonStandardCombinations) return 0;
            return 2500;
        });
        _calculations.Add(ScoredCombination.ThreePairs, values =>
        {
            return 1500;
        });
        _calculations.Add(ScoredCombination.Straight, values =>
        {
            return 3000;
        });
    }
}

public class ScoringRules
{
    public bool AllowNonStandardCombinations { get; set; }
    public bool SixOnesWins { get; set; }

    public static ScoringRules Standard { get; } = new ScoringRules() { AllowNonStandardCombinations = false, SixOnesWins = false };
    public static ScoringRules Alternate { get; } = new ScoringRules() { AllowNonStandardCombinations = true, SixOnesWins = true };
}