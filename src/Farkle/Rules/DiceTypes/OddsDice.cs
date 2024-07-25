namespace Farkle.Rules.DiceTypes;

public class OddsDice : DiceBase
{
    public override string Name => "Odds";
    public override string Description => "Only rolls odd numbers.";

    public OddsDice()
    {
        SetWeights(1f / 3f, 0f, 1f / 3f, 0f, 1f / 3f, 0f);
    }
}