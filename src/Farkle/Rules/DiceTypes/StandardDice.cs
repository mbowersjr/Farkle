using System;

namespace Farkle.Rules.DiceTypes;

public class StandardDice : DiceBase
{
    public override string Name => "Standard";
    public override string Description => "Standard dice with equally probable sides.";

    public StandardDice()
    {
        SetWeights(1f / 6f, 1f / 6f, 1f / 6f, 1f / 6f, 1f / 6f, 1f / 6f);
    }
}