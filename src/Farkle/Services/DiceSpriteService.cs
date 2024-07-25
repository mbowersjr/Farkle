using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Farkle.Services.UI;

namespace Farkle.Services;

public class DiceSpriteService : SimpleGameComponent
{
    private Texture2D _spriteSheet;
    private Texture2D[] _sprites;
    private IntPtr[] _imagePointers;

    public static Point SpriteSize = new Point(110, 110);
    public static Vector2 SpriteOrigin = new Vector2(SpriteSize.X * 0.5f, SpriteSize.Y * 0.5f);

    private readonly GameMain _game;
    private ImGuiRenderer _imGuiRenderer;

    public DiceSpriteService(GameMain game)
    {
        _game = game;
    }

    public override void Initialize()
    {
        _imGuiRenderer = _game.Services.GetService<ImGuiRenderer>();
        
        LoadContent();
    }

    protected override void LoadContent()
    {
        string imageName = "dice-110";
        int imageSize = int.Parse(imageName.Split('-')[1]);

        SpriteSize = new Point(imageSize, imageSize);
        SpriteOrigin = new Vector2(SpriteSize.X * 0.5f, SpriteSize.Y * 0.5f);

        _spriteSheet = _game.Content.Load<Texture2D>($"Images/{imageName}");
        SplitSpriteSheet(_spriteSheet);
    }

    public IntPtr GetDiceImagePtr(int number)
    {
        if (number < 1 || number > 6)
            throw new ArgumentOutOfRangeException(nameof(number), number, "Value must be between 1 and 6.");

        return _imagePointers[number - 1];
    }

    public Texture2D GetDiceSprite(int number)
    {
        if (number < 1 || number > 6)
            throw new ArgumentOutOfRangeException(nameof(number), number, "Value must be between 1 and 6.");

        return _sprites[number - 1];
    }

    private void SplitSpriteSheet(Texture2D spriteSheet)
    {
        _sprites = new Texture2D[6];
        _imagePointers = new IntPtr[_sprites.Length];

        for (var number = 1; number <= 6; number++)
        {
            var sourceRect = GetSpriteSourceRectangle(number);
            var spriteData = new Color[SpriteSize.X * SpriteSize.Y];
            spriteSheet.GetData(0, sourceRect, spriteData, 0, spriteData.Length);

            var sprite = new Texture2D(_game.GraphicsDevice, SpriteSize.X, SpriteSize.Y);
            sprite.SetData(spriteData);

            _sprites[number - 1] = sprite;
            _imagePointers[number - 1] = _imGuiRenderer.BindTexture(sprite);
        }
    }

    private Rectangle GetSpriteSourceRectangle(int number)
    {
        if (number < 1 || number > 6)
            throw new ArgumentOutOfRangeException(nameof(number), number, "Value must be between 1 and 6.");

        return new Rectangle((number - 1) * SpriteSize.X, 0, SpriteSize.X, SpriteSize.Y);
    }

    protected override void UnloadContent()
    {
        for (var i = 0; i < _sprites.Length; i++) 
            _sprites[i]?.Dispose();

        _sprites = null;

        base.UnloadContent();
    }

    public override void Update(GameTime gameTime)
    {
    }
}