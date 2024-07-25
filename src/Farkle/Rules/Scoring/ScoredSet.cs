using System;
using System.Collections.Generic;
using Farkle.Rules.DiceTypes;

namespace Farkle.Rules.Scoring;

public class ScoredSet
{
    public List<DiceBase> Dice { get; set; }
    public int Score { get; set; }

    public ScoredSet(IEnumerable<DiceBase> dice, int score)
    {
        if (dice == null)
            throw new ArgumentNullException(nameof(dice));

        Dice = new List<DiceBase>(dice);
        Score = score;
    }
}