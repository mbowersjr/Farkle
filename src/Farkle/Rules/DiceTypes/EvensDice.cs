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