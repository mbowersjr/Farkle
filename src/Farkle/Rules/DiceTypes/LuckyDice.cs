using System;

namespace Farkle.Rules.DiceTypes;

public class LuckyDice : DiceBase
{
    public override string Name => "Lucky";
    public override string Description => "Twice as likely to roll 1s and 5s.";

    public LuckyDice()
    {
        SetWeights(1f / 3f, 1f / 6f, 1f / 6f, 1f / 6f, 1f / 3f, 1f / 6f);
    }
}