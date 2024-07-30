using System;
using System.Collections.Generic;
using System.Text;
using Farkle.Services.UI;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Farkle.Services;

public class InterfaceManager : SimpleDrawableGameComponent
{
    private List<string> _logEntries = new List<string>();

    private readonly GameMain _game;

    private SpriteBatch _spriteBatch;

    public const float ViewportPadding = 20f;
    private bool _needsUpdated = false;
    private Vector2 _logDrawPosition;
    private RectangleF _logDrawBounds;
    private SpriteFontBase _logFont;
    private StringBuilder _logStringBuilder = new StringBuilder();
    private readonly ResourceManager _resourceManager;

    public InterfaceManager(GameMain game)
    {
        _game = game;
        _resourceManager = _game.Services.GetService<ResourceManager>();
    }

    private void CalculateLogDrawBounds()
    {
        var logDrawBoundsHeight = 200f;
        var logDrawBoundsWidth = _game.GraphicsDevice.Viewport.Bounds.Width - ViewportPadding - ViewportPadding;
            
        _logDrawBounds = new RectangleF(
            ViewportPadding, 
            _game.GraphicsDevice.Viewport.Bounds.Bottom - logDrawBoundsHeight - ViewportPadding,
            logDrawBoundsWidth, 
            logDrawBoundsHeight
        );
    }

    public override void Initialize()
    {
        _game.Window.ClientSizeChanged += OnClientSizeChanged;
            
        base.Initialize();            
    }

    private void OnClientSizeChanged(object sender, EventArgs args)
    {
        CalculateLogDrawBounds();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
        
        _logFont = _resourceManager.GetFont(GuiFontProvider.GetFontResourceName(GuiFontStyle.MonoRegular), GuiFontProvider.NormalFontSize);
        
        CalculateLogDrawBounds();
            
        base.LoadContent();
    }

    protected override void UnloadContent()
    {
        _logEntries.Clear();

        base.UnloadContent();
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
               
    private void RebuildLogMessagesString() {
        if (!_needsUpdated)
            return;

        _logStringBuilder.Clear();

        for (int i = 0; i < _logEntries.Count; i++)
        {
            _logStringBuilder.AppendLine(_logEntries[i]);
        }
    }

    private void RedrawLogMessages(SpriteBatch spriteBatch)
    {
        if (_logDrawBounds == RectangleF.Empty)
            return;

        RebuildLogMessagesString();

        _logDrawPosition = _logDrawBounds.Position;

        var stringDimensions = _logFont.MeasureString(_logStringBuilder);
        if (stringDimensions.Y > _logDrawBounds.Height)
        {
            _logDrawPosition.Y -= (stringDimensions.Y - _logDrawBounds.Height);
        }

        spriteBatch.DrawString(
            font: _logFont,
            text: _logStringBuilder,
            position: _logDrawPosition,
            color: Color.White
            //textStyle: TextStyle.None,
            //effect: FontSystemEffect.None,
            //lineSpacing: 0f,
            //characterSpacing: 0f
        );

        if (GameMain.DrawDebugLines)
        {
            spriteBatch.DrawRectangle(
                rectangle: _logDrawBounds,
                color: Color.White,
                thickness: 1f,
                layerDepth: 0f
            );
        }
    }
    public override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Immediate,
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            transformMatrix: _game.Camera.GetViewMatrix()
        );

        RedrawLogMessages(_spriteBatch);

        _spriteBatch.End();
    }
    
    public override void Dispose()
    {
        _game.Window.ClientSizeChanged -= OnClientSizeChanged;

        _logEntries.Clear();

        base.Dispose();
    }
                
    public void Log(string message)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));

        _logEntries.Add(message);
        _needsUpdated = true;
    }

    public void ClearLog()
    {
        _logEntries.Clear();
        _needsUpdated = true;
    }

}