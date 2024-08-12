using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using Microsoft.Xna.Framework;
using Farkle.Rules.DiceTypes;
using Farkle.Services;
using MonoGame.Extended;
using System.Diagnostics;

namespace Farkle;

public class DiceSprite : IComparable<DiceSprite>
{
    private Vector2 _position = Vector2.Zero;
    public Vector2 Position
    {
        get => _position;
        set
        {
            if (_position == value) return;

            _position = value;
            Bounds = new RectangleF(
                x: _position.X, 
                y: _position.Y, 
                width: DiceSpriteService.SpriteSize.X, 
                height: DiceSpriteService.SpriteSize.Y
            );
        }
    }
    public RectangleF Bounds { get; private set; } = RectangleF.Empty;

    public float Rotation { get; set; } = 0f;
    
    public DiceState State { get; set; } = DiceState.None;

    public bool Selected
    {
        get => State.HasFlag(DiceState.Selected);
        set
        {
            if (value)
                State |= DiceState.Selected;
            else
                State &= ~ DiceState.Selected;
        }
    }

    public bool Available
    {
        get => State.HasFlag(DiceState.Available);
        set
        {
            if (value)
                State |= DiceState.Available;
            else
                State &= ~ DiceState.Available;
        }
    }
    public bool Is(DiceState state) => State.HasFlag(state);

    public DiceBase Dice { get; set; }
    public int Value
    {
        get => Dice.Value;
        set => Dice.Value = value;
    }

    public DiceSprite([NotNull] DiceBase dice)
    {
        ArgumentNullException.ThrowIfNull(dice);
        Dice = dice;
    }

    public void ChangeDiceType(Type diceType)
    {
        ArgumentNullException.ThrowIfNull(diceType);
        
        if (!diceType.IsSubclassOf(typeof(DiceBase)))
            throw new ArgumentException("Type must be derived from DiceBase", nameof(diceType));

        if (Dice.GetType() == diceType) return;

        Dice = (DiceBase)Activator.CreateInstance(diceType);
    }

    public void ChangeDiceType<TDice>() where TDice : DiceBase, new()
    {
        ChangeDiceType(typeof(TDice));
    }

    public static DiceSprite Create(Type diceType)
    {
        DiceBase dice = (DiceBase)Activator.CreateInstance(diceType);
        DiceSprite diceSprite = new DiceSprite(dice);
        return diceSprite;
    }

    public static DiceSprite Create<TDice>() where TDice : DiceBase, new()
    {
        return Create(typeof(TDice));
    }

    #region IComparer

    private sealed class ValueNameStateRelationalComparer : IComparer<DiceSprite>
    {
        public int Compare(DiceSprite x, DiceSprite y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (ReferenceEquals(null, y)) return 1;
            if (ReferenceEquals(null, x)) return -1;
            var valueNameComparison = x.Dice.CompareTo(y.Dice);
            if (valueNameComparison != 0) return valueNameComparison;
            return x.State.CompareTo(y.State);
        }
    }

    public static IComparer<DiceSprite> ValueNameStateComparer { get; } = new ValueNameStateRelationalComparer();

    public int CompareTo(DiceSprite other)
    {
        return ValueNameStateComparer.Compare(this, other);
    }

    #endregion
}