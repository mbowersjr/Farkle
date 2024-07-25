using System;

namespace Farkle.Rules.DiceTypes;

public class EvensDice : DiceBase
{
    public override string Name => "Evens";
    public override string Description => "Only rolls even numbers.";

    public EvensDice()
    {
        SetWeights(0f, 1f / 3f, 0f, 1f / 3f, 0f, 1f / 3f);
    }
}
public class OddsDice : DiceBase
{
    public override string Name => "Odds";
    public override string Description => "Only rolls odd numbers.";

    public OddsDice()
    {
        SetWeights(1f / 3f, 0f, 1f / 3f, 0f, 1f / 3f, 0f);
    }
}