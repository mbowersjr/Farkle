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
/// <list type="table">
/// <listheader>    
///     <description>Pair Scoring</description>
/// </listheader>
///     <item> <term> Ones </term> <description> 100 </description> </item>
///     <item> <term> Fives </term> <description> 50 </description> </item>
///     <item> <term> 3 Ones </term> <description> 1000 </description> </item>
///     <item> <term> 3 Twos </term> <description> 200 </description> </item>
///     <item> <term> 3 Threes </term> <description> 300 </description> </item>
///     <item> <term> 3 Fours </term> <description> 400 </description> </item>
///     <item> <term> 3 Fives </term> <description> 500 </description> </item>
///     <item> <term> 3 Sixes </term> <description> 600 </description> </item>
///     <item> <term> 3 Pairs </term> <description> 1500 </description> </item>
///     <item> <term> 2 Triplets </term> <description> 2500 </description> </item>
///     <item> <term> 4 of a Kind </term> <description> 1000 </description> </item>
///     <item> <term> 5 of a Kind </term> <description> 2000 </description> </item>
///     <item> <term> 6 of a Kind </term> <description> 3000 </description> </item>
///     <item> <term> 4 of a Kind and a Pair </term> <description> 1500 </description> </item>
///     <item> <term> Straight (1-6) </term> <description> 1500 </description> </item>
///     <item> <term> 3 Fails </term> <description> -1000 </description> </item>
/// </list>
/// </remarks>
public class ScoringService
{
    public ScoringService()
    {
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
                score = 100;
            }
            else if (dice[0].Value == 5)
            {
                score = 50;
            }
        }
        else if (dice.Count == 3 && distinctValues.Count == 1)
        {
            if (dice[0].Value == 1)
            {
                score = 1000;
            }
            else
            {
                score = dice[0].Value * 100;
            }
        }
        else if (dice.Count == 4 && distinctValues.Count == 1)
        {
            // 4 of a kind
            score = 1000;
        }
        else if (dice.Count == 5 && distinctValues.Count == 1)
        {
            // 5 of a kind
            score = 2000;
        }
        else if (dice.Count == 6 && distinctValues.Count == 1)
        {
            // 6 of a kind
            score = 3000;
        }
        else if (dice.Count == 6 && distinctValues.Count == 2)
        {
            var numberOfFirstValue = dice.Count(x => x.Value == distinctValues[0]);
                                
            if (numberOfFirstValue == 3)
            {
                // 2 triplets
                score = 2500;
            }
            else if (numberOfFirstValue == 2 || numberOfFirstValue == 4)
            {
                // 4 of a kind and a pair
                score = 1500;
            }
        }
        else if (dice.Count == 6 && distinctValues.Count == 3)
        {                   
            // 3 pairs
            score = 1500;                
        }
        else if (dice.Count == 6 && distinctValues.Count == 6)
        {
            // straight 1-6
            score = 1500;
        }

        var scoredSet = new ScoredSet(dice, score);
        return scoredSet;
    }
}