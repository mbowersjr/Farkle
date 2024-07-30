using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Farkle.Rules.DiceTypes;
using Farkle.Rules.Scoring;
using MonoGame.Extended;

namespace Farkle.Services;

public class DiceManager
{
    private Dictionary<DiceBase, DiceSprite> _dice = new Dictionary<DiceBase, DiceSprite>();
        
    private ScoringService _scoringService;
    
    private readonly GameMain _game;
    
    public DiceManager(GameMain game)
    {
        _game = game;

        Initialize();
    }

    protected void Initialize()
    {
        _scoringService = _game.Services.GetService<ScoringService>();
    }

    public void AddDice<T>(int count) where T : DiceBase, new()
    {
        for (var i = 0; i < count; i++)
        {
            AddDice<T>();
        }
    }

    public void AddDice<T>() where T : DiceBase, new()
    {
        T dice = Activator.CreateInstance<T>();

        DiceSprite diceSprite = new DiceSprite(dice);
        diceSprite.State = DiceState.Available;

        _dice.Add(dice, diceSprite);
        
    }

    public (DiceBase DiceBase, DiceSprite DiceSprite) ChangeDiceType(DiceBase oldDie, Type type)
    {
        ArgumentNullException.ThrowIfNull(oldDie);
        ArgumentNullException.ThrowIfNull(type);
        
        if (!type.IsSubclassOf(typeof(DiceBase)))
            throw new ArgumentException("Type must be of DiceBase");

        if (oldDie.GetType() == type)
            return (oldDie, _dice[oldDie]);

        DiceBase newDie = (DiceBase)Activator.CreateInstance(type);
        
        if (newDie == null)
            throw new InvalidOperationException("Couldn't create instance of of dice type.");

        DiceSprite diceSprite = new DiceSprite(newDie);
        diceSprite.State = DiceState.Available;

        _dice.Remove(oldDie);
        _dice.Add(newDie, diceSprite);
        
        return (newDie, diceSprite);
    }

    public void ChangeDiceType<T>(DiceBase dice) where T : DiceBase
    {

        ChangeDiceType(dice, typeof(T));
    }

    public void RemoveDice<T>(T dice) where T : DiceBase
    {
        ArgumentNullException.ThrowIfNull(dice);

        _dice.Remove(dice);
    }

    public IList<DiceBase> GetDiceExact(DiceState state)
    {
        return GetDiceSpritesExact(state).Select(x => x.Dice).ToList();
    }

    public IList<DiceBase> GetDice(params DiceState[] states)
    {
        return GetDiceSprites(states).Select(x => x.Dice).ToList();
    }

    public IList<DiceSprite> GetDiceSpritesExact(params DiceState[] states)
    {
        if (states.Length == 0)
            return new List<DiceSprite>();

        if (states.Contains(DiceState.All))
            return _dice.Values.ToList();

        List<DiceSprite> diceSprites = new List<DiceSprite>();
        foreach (var state in states)
        {
            var matching = _dice.Values.Where(x => x.State == state && !diceSprites.Contains(x));
            diceSprites.AddRange(matching);
        }

        return diceSprites;
    }

    public IList<DiceSprite> GetDiceSprites(params DiceState[] states)
    {
        if (states.Length == 0)
            return new List<DiceSprite>();

        if (states.Contains(DiceState.All))
            return _dice.Values.ToList();

        List<DiceSprite> diceSprites = new List<DiceSprite>();
        foreach (var state in states)
        {
            var matching = _dice.Values.Where(x => x.State.HasFlag(state) && !diceSprites.Contains(x));
            diceSprites.AddRange(matching);
        }

        return diceSprites;
    }

    public void Roll()
    {
        var availableDice = GetDiceSpritesExact(DiceState.Available);
        
        foreach (var dice in availableDice)
        {
            dice.Dice.Roll();
        }
    }

    public void Score()
    {
        var selected = GetDiceSprites(DiceState.Selected);

        ScoredSet scoredSet = _scoringService.CalculateScore(selected);

        if (scoredSet.Combination == ScoredCombination.None)
        {
            foreach (DiceSprite dice in GetDiceSprites(DiceState.All))
            {
                dice.State = DiceState.Scored;
            }

            return;
        }
        
        foreach (DiceSprite dice in selected)
        {
            dice.State = DiceState.Scored;
        }
    }
}

[Flags]
public enum DiceState
{
    Available = 1 << 0,
    Selected = 1 << 1,
    Scored = 1 << 2,

    All = Available | Selected | Scored,
    
    None = 0
}