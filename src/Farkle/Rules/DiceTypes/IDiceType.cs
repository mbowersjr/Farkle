namespace Farkle.Rules.DiceTypes;

public interface IDiceType
{
    static abstract string Name { get; }
    static abstract string Description { get; }
}