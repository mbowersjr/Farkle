using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Farkle.Rules.DiceTypes;
using Farkle.Rules.Scoring;
using MonoGame.Extended;

namespace Farkle.Services;

public class DiceManager
{
    private Dictionary<DiceSprite, DiceBase> _dice = new Dictionary<DiceSprite, DiceBase>();
    
    private HashSet<DiceSprite> _diceSprites = new HashSet<DiceSprite>();

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

    public void AddDice(int count, Type diceType)
    {
        for (int i = 0; i < count; i++)
        {
            AddDice(diceType);
        }
    }

    public void AddDice<T>(int count) where T : DiceBase, new()
    {
        for (var i = 0; i < count; i++)
        {
            AddDice<T>();
        }
    }

    public void AddDice(Type diceType)
    {
        ArgumentNullException.ThrowIfNull(diceType);
        
        if (!diceType.IsSubclassOf(typeof(DiceBase)))
            throw new ArgumentException("Type must be derived from DiceBase", nameof(diceType));

        DiceSprite diceSprite = DiceSprite.Create(diceType);
        diceSprite.State = DiceState.Available;

        _diceSprites.Add(diceSprite);
        _dice.Add(diceSprite, diceSprite.Dice);
    }

    public void AddDice<TDice>() where TDice : DiceBase, new()
    {
        AddDice(typeof(TDice));
    }

    public void ChangeDiceType([NotNull]DiceSprite diceSprite, [NotNull]Type newType)
    {
        ArgumentNullException.ThrowIfNull(diceSprite);
        ArgumentNullException.ThrowIfNull(newType);
        diceSprite.ChangeDiceType(newType);
    }
    public void ChangeDiceType<TDice>([NotNull]DiceSprite diceSprite) where TDice : DiceBase, new()
    {
        ArgumentNullException.ThrowIfNull(diceSprite);
        diceSprite.ChangeDiceType<TDice>();
    }

    public void RemoveDice(DiceSprite dice)
    {
        ArgumentNullException.ThrowIfNull(dice);
        
        _diceSprites.Remove(dice);
        _dice.Remove(dice);
    }

    public void SetAllDiceStates(DiceState state)
    {
        foreach (DiceSprite diceSprite in _diceSprites)
        {
            diceSprite.State = state;
        }
    }

    public IList<DiceSprite> GetDiceSpritesExact([NotNull]params DiceState[] states)
    {
        if (states == null || states.Length == 0 || states.Contains(DiceState.All))
            return _diceSprites.ToList();

        HashSet<DiceSprite> results = new HashSet<DiceSprite>();
        foreach (DiceState state in states)
        {
            var matching = _diceSprites.Where(x => x.State == state);
            results.UnionWith(matching);
        }

        return results.ToList();
    }

    public IList<DiceSprite> GetDiceSprites([NotNull]params DiceState[] states)
    {
        if (states == null || states.Length == 0 || states.Contains(DiceState.All))
            return _diceSprites.ToList();

        HashSet<DiceSprite> results = new HashSet<DiceSprite>();
        foreach (DiceState state in states)
        {
            var matching = _diceSprites.Where(x => x.State.HasFlag(state));
            results.UnionWith(matching);
        }

        return results.ToList();
    }

    //public void RollValues(params int[] values)
    //{
    //    var dice = GetDiceSpritesExact(DiceState.All);
        
    //    for (int i = 0; i < values.Length; i++)
    //    {
    //        dice[i].State = DiceState.Available;
    //        dice[i].Value = values[i];
    //    }

    //    for (int i = values.Length; i < dice.Count; i++)
    //    {
    //        dice[i].State = DiceState.None;
    //    }
    //}

    public void Roll()
    {
        var availableDice = GetDiceSpritesExact(DiceState.Available);

        foreach (var dice in availableDice)
        {
            dice.Dice.Roll();
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