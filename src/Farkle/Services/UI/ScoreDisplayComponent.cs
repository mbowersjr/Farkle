using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FontStashSharp;
using MonoGame.Extended;
using MonoGame.Framework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Farkle.Services.UI;

public class ScoreDisplayComponent : SimpleDrawableGameComponent
{
    private GameMain _game;

    private Vector2 _position;
    private StringBuilder _stringBuilder = new StringBuilder();
        
    private SpriteFontBase _font;
    private SpriteBatch _spriteBatch;

    private GameStateManager _gameStateManager;
    private ResourceManager _resourceManager;
                
    private SizeF _scoreDisplaySize = new SizeF(200f, 200f);

    public bool NeedsUpdated { get; set; }

    public ScoreDisplayComponent(GameMain game)
    {
        _game = game;
    }

    private void UpdateString()
    {
        _stringBuilder.Clear();

        foreach (var score in _gameStateManager.ScoredSets)
        {
            foreach (var die in score.Dice)
            {
                _stringBuilder.AppendFormat("{0} ", die.Value);
            }

            _stringBuilder.AppendFormat(" = {0}\n", score.Score);
        }
    }

                
    private void CalculatePosition()
    {
        var viewport = _game.GraphicsDevice.Viewport.Bounds;
        
        _position = new Vector2(
            viewport.Width - _scoreDisplaySize.Width - InterfaceManager.ViewportPadding,
            viewport.Height - _scoreDisplaySize.Height - InterfaceManager.ViewportPadding
        );
    }

    public override void Draw(GameTime gameTime)
    {
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        if (NeedsUpdated) 
            UpdateString();

        spriteBatch.DrawString(
            font: _font,
            text: _stringBuilder,
            position: _position,
            color: Color.White
        );
    }

    public override void Initialize()
    {
        _gameStateManager = _game.Services.GetService<GameStateManager>();
        _resourceManager = _game.Services.GetService<ResourceManager>();
        
        CalculatePosition();
            
        base.Initialize();
    }
        
    //private TimeSpan _updateInterval = TimeSpan.FromSeconds(1);
    //private TimeSpan _updateElapsed = TimeSpan.Zero;
    public override void Update(GameTime gameTime)
    {
        //_updateElapsed += gameTime.ElapsedGameTime;
        //if (_updateElapsed >= _updateInterval)
        //{
        //    _updateElapsed -= _updateInterval;
        //}
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        _font = _resourceManager.GetFont(GuiFontProvider.GetFontResourceName(GuiFontStyle.Regular), GuiFontProvider.NormalFontSize);

        base.LoadContent();
    }

    protected override void UnloadContent()
    {            
        base.UnloadContent();
    }
                
    public override void Dispose()
    {
        _stringBuilder.Clear();
        _spriteBatch?.Dispose();

        base.Dispose();
    }
}