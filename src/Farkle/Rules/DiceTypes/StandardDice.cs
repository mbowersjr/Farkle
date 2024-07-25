using System;

namespace Farkle.Rules.DiceTypes;

public class StandardDice : DiceBase
{
    public override string Name => "Standard";
    public override string Description => "Standard dice that with equally probable rolls.";

    public StandardDice()
    {
        SetWeights(1f / 6f, 1f / 6f, 1f / 6f, 1f / 6f, 1f / 6f, 1f / 6f);
    }
}