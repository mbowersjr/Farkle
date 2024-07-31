using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Farkle.Rules.DiceTypes;
using Point = Microsoft.Xna.Framework.Point;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SizeF = MonoGame.Extended.SizeF;

namespace Farkle;

public static class Extensions
{
    public static Microsoft.Xna.Framework.Vector2 ToVector2(this System.Numerics.Vector2 vector2) => new(vector2.X, vector2.Y);
    public static System.Numerics.Vector2 ToVector2(this Microsoft.Xna.Framework.Vector2 vector2) => new(vector2.X, vector2.Y);
    public static MonoGame.Extended.RectangleF ToRectangleF(this Rectangle rectangle) => new(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
    public static SizeF ToSizeF(this Point point) => new SizeF(point.X, point.Y);
    public static SizeF ToSizeF(this Microsoft.Xna.Framework.Vector2 vector2) => new(vector2.X, vector2.Y);
    public static SizeF ToSizeF(this PointF pointf) => new(pointf.X, pointf.Y);
    public static PointF ToPointF(this SizeF sizef) => new(sizef.Width, sizef.Height);
    public static PointF ToPointF(this Point point) => new(point.X, point.Y);


    public static int[] GetValues(this IEnumerable<DiceBase> dice) => dice.Select(x => x.Value).OrderBy(x => x).ToArray();
    public static int[] GetValues(this IEnumerable<DiceSprite> dice) => dice.Select(x => x.Dice.Value).OrderBy(x => x).ToArray();

}