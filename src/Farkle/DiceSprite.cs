using Microsoft.Xna.Framework;
using Farkle.Rules.DiceTypes;
using Farkle.Services;
using MonoGame.Extended;

namespace Farkle;

public class DiceSprite
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

    public float Rotation { get; set; } = 0f;
    
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

    public DiceState State { get; set; } = DiceState.None;
    public DiceBase Dice { get; set; }
    public RectangleF Bounds { get; private set; } = RectangleF.Empty;
    public bool Is(DiceState state) => State.HasFlag(state);

    public DiceSprite(DiceBase dice)
    {
        Dice = dice;
    }
}