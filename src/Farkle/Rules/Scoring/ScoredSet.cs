using System;
using System.Collections.Generic;
using System.Linq;
using Farkle.Rules.DiceTypes;

namespace Farkle.Rules.Scoring;

public class ScoredSet
{
    public int[] Values { get; set; }
    public int Score { get; private set; } = 0;
    public int Turn { get; set; } = 0;
    public ScoredCombination Combination { get; private set; } = ScoredCombination.None;

    private ScoredSet()
    {
    }

    public ScoredSet(IEnumerable<DiceBase> dice, int score, ScoredCombination combination, int turn = 0)
    {
        ArgumentNullException.ThrowIfNull(dice);

        Values = dice.OrderBy(x => x.Value).Select(x => x.Value).ToArray();
        Score = score;
        Combination = combination;
        Turn = turn;
    }

    private sealed class ScoredSetEqualityComparer : IEqualityComparer<ScoredSet>
    {
        public bool Equals(ScoredSet x, ScoredSet y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return Equals(x.Values, y.Values) && x.Score == y.Score && x.Turn == y.Turn && x.Combination == y.Combination;
        }

        public int GetHashCode(ScoredSet obj)
        {
            return HashCode.Combine(obj.Values, obj.Score, obj.Turn, (int)obj.Combination);
        }
    }

    public static IEqualityComparer<ScoredSet> ScoredSetComparer { get; } = new ScoredSetEqualityComparer();

    public static ScoredSet None { get; } = new ScoredSet();
}