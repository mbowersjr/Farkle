using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Farkle.Services;

public class TextureResource : IDisposable
{
    public string Name { get; private set; }
    public Texture2D Texture { get; private set; }
    public Vector2 Origin { get; private set; }

    public TextureResource(string name, Texture2D texture)
    {
        Name = name;
        Texture = texture;
        Origin = Texture.Bounds.Center.ToVector2();
    }

    public void Dispose()
    {
        Texture?.Dispose();
    }
}