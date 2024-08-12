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
///     <item> <term> Three of a Kind (except 1s) </term> <description> N * 100 </description> </item>
///     <item> <term> Four of a Kind </term> <description> 2x Three of a Kind </description> </item>
///     <item> <term> Five of a Kind </term> <description> 2x Four of a Kind score </description> </item>
///     <item> <term> Six of a Kind </term> <description> 2x Five of a Kind score </description> </item>
///     <item> <term> Straight (1-6) </term> <description> 1500 </description> </item>
///     <item> <term> Partial Straight (1-5) </term> <description> 500 </description> </item>
///     <item> <term> Partial Straight (2-6) </term> <description> 750 </description> </item>
/// </list>
/// </remarks>
public class ScoringService
{
    public ScoringRules Rules { get; set; }

    public ScoringService()
        : this(ScoringRules.Standard)
    {
    }
    
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
        else if (dice.Count == 6 && distinctValues.Count == 6)
        {
            // Straight 1-6
            return Calculate(dice, ScoredCombination.Straight);
        }
        else if (dice.Count == 5 && distinctValues.Count == 5)
        {
            if (dice[0].Value == 1)
            {
                // Partial Straight (1-5)
                return Calculate(dice, ScoredCombination.PartialStraightLow);
            }
            if (dice[0].Value == 2)
            {
                // Partial Straight (2-6)
                return Calculate(dice, ScoredCombination.PartialStraightHigh);
            }
        }
        //else if (dice.Count == 5 && distinctValues.Count == 5)
        //{
        //    return Calculate(dice, ScoredCombination.PartialStraight);
        //}

        return ScoredSet.None;
    }

    private readonly Dictionary<ScoredCombination, Func<int[], int>> _calculations = new Dictionary<ScoredCombination, Func<int[], int>>();

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
            return values[0] switch
            {
                1 => 2000,
                _ => values[0] * 100 * 2    // 2x Three of a kind score
            };
        });
        _calculations.Add(ScoredCombination.FiveOfAKind, values =>
        {
            return values[0] switch
            {
                1 => 4000,
                _ => values[0] * 100 * 4    // 2x Four of a kind score
            };
        });
        _calculations.Add(ScoredCombination.SixOfAKind, values =>
        {
            return values[0] switch
            {
                1 => 8000,
                _ => values[0] * 100 * 8    // 2x Five of a kind score
            };
        });
        _calculations.Add(ScoredCombination.Straight, values => 1500);
        _calculations.Add(ScoredCombination.PartialStraightLow, values => 500);
        _calculations.Add(ScoredCombination.PartialStraightHigh, values => 750);
        //_calculations.Add(ScoredCombination.PartialStraight, values =>
        //{
        //    return values[0] switch
        //    {
        //        1 => 500,   // 1 2 3 4 5
        //        2 => 750    // 2 3 4 5 6
        //    };
        //});
    }
}