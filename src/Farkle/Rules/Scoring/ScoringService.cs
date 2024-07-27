using System;
using System.Linq;
using System.Collections.Generic;
using Farkle.Rules.DiceTypes;

namespace Farkle.Rules.Scoring;

public interface IScoringService
{
    ScoredSet CalculateScore(IList<DiceSprite> diceSprites);
    ScoredSet CalculateScore(IList<DiceBase> dice);
}

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
///     <item> <term> Four of a Kind </term> <description> 1000 </description> </item>
///     <item> <term> Five of a Kind </term> <description> 2000 </description> </item>
///     <item> <term> Six of a Kind </term> <description> 3000 </description> </item>
///     <item> <term> Straight (1-6) </term> <description> 2500 </description> </item>
/// </list>
/// 
/// <para>Non-Standard Rules</para>
/// <list type="table">
///     <item> <term> Two Triplets </term> <description> 2500 </description> </item>
///     <item> <term> Four of a Kind and a Pair </term> <description> 1500 </description> </item>
/// </list>
/// </remarks>
public class ScoringService
{
    public bool AllowNonStandardCombinations { get; set; }

    public ScoringService(bool allowNonStandardCombinations = false)
    {
        AllowNonStandardCombinations = allowNonStandardCombinations;
    }

    public ScoredSet CalculateScore(IList<DiceSprite> diceSprites)
    {
        var dice = diceSprites.Select(x => x.Dice).ToList();
        return CalculateScore(dice);
    }

    public ScoredSet CalculateScore(IList<DiceBase> dice)
    {
        ArgumentNullException.ThrowIfNull(dice);

        int score = 0;
        
        var values = dice.Select(x => x.Value).OrderBy(x => x).ToList();
        var distinctValues = values.Distinct().ToList();

        if (dice.Count == 1)
        {
            if (dice[0].Value == 1)
            {
                // 1
                score = 100;
            }
            else if (dice[0].Value == 5)
            {
                // 5
                score = 50;
            }
        }
        else if (dice.Count == 3 && distinctValues.Count == 1)
        {
            if (dice[0].Value == 1)
            {
                // Three 1s
                score = 1000;
            }
            else
            {
                // Three of a kind (except 1s)
                score = dice[0].Value * 100;
            }
        }
        else if (dice.Count == 4 && distinctValues.Count == 1)
        {
            // Four of a kind
            score = 1000;
        }
        else if (dice.Count == 5 && distinctValues.Count == 1)
        {
            // Five of a kind
            score = 2000;
        }
        else if (dice.Count == 6 && distinctValues.Count == 1)
        {
            // Six of a kind
            score = 3000;
        }
        else if (dice.Count == 6 && distinctValues.Count == 2 && AllowNonStandardCombinations)
        {
            var numberOfFirstValue = dice.Count(x => x.Value == distinctValues[0]);

            if (numberOfFirstValue == 3)
            {
                // Two triplets
                score = 2500;
            }
            else if (numberOfFirstValue == 2 || numberOfFirstValue == 4)
            {
                // Four of a kind and a pair
                score = 1500;
            }
        }
        else if (dice.Count == 6 && distinctValues.Count == 3)
        {                   
            // Three pairs
            score = 1500;
        }
        else if (dice.Count == 6 && distinctValues.Count == 6)
        {
            // Straight 1-6
            score = 2500;
        }

        var scoredSet = new ScoredSet(dice, score);
        return scoredSet;
    }
}