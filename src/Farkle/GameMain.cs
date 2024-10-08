﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Num = System.Numerics;
using MonoGame.Extended;
using MonoGame.Extended.Input;
using MonoGame.Extended.Input.InputListeners;
using MonoGame.Extended.VectorDraw;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.Graphics;
using ImGuiNET;
using Farkle.Services;
using Farkle.Services.UI;
using Farkle.Rules;
using Farkle.Rules.DiceTypes;
using Farkle.Rules.Scoring;
using System.Xml.Linq;

namespace Farkle;

public class GameMain : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;


    private DiceSpriteService _diceSpriteService;
    private InputManager _inputManager;
    private InterfaceManager _interfaceManager;
    private ResourceManager _resourceManager;
    private DiceManager _diceManager;
    private ScoringService _scoringService;
    private ImGuiRenderer _imGuiRenderer;
    private GameStateManager _gameStateManager;
    private GuiFontProvider _fontProvider;
    private GuiService _guiService;

    private ViewportAdapter _viewportAdapter;
    private Camera<Vector2> _camera;
    public Camera<Vector2> Camera => _camera;

    public static bool DrawDebugLines = false;

    public static readonly FastRandom Random = new FastRandom(DateTime.Now.Millisecond);

    public GameMain()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = false;
        
        Window.Title = "Farkle";
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += WindowOnClientSizeChanged;
    }

    private void WindowOnClientSizeChanged(object sender, EventArgs e)
    {
        Window.ClientSizeChanged -= WindowOnClientSizeChanged;
        
        InitializeViewport(true);

        Window.ClientSizeChanged += WindowOnClientSizeChanged;
    }

    protected override void Initialize()
    {
        InitializeViewport();
            
        _resourceManager = new ResourceManager(this);
        _resourceManager.Load();
        Services.AddService(_resourceManager);

        _interfaceManager = new InterfaceManager(this);
        Components.Add(_interfaceManager);
        Services.AddService(_interfaceManager);

        _inputManager = new InputManager(this);
        _inputManager.Keyboard.KeyPressed += OnKeyPressed;
        _inputManager.Mouse.MouseClicked += OnMouseClicked;
        Components.Add(_inputManager);
        Services.AddService(_inputManager);

        _diceManager = new DiceManager(this);
        Services.AddService(_diceManager);

        _scoringService = new ScoringService(ScoringRules.Standard);
        Services.AddService(_scoringService);

        _fontProvider = new GuiFontProvider(_resourceManager);
        Services.AddService(_fontProvider);

        _imGuiRenderer = new ImGuiRenderer(this);
        Services.AddService(_imGuiRenderer);

        _guiService = new GuiService(this);
        Components.Add(_guiService);
        Services.AddService(_guiService);

        _diceSpriteService = new DiceSpriteService(this);
        Components.Add(_diceSpriteService);
        Services.AddService(_diceSpriteService);
        
        _gameStateManager = new GameStateManager(this);
        Components.Add(_gameStateManager);
        Services.AddService(_gameStateManager);

        _debugLineColor = new Color(ImGui.GetStyle().Colors[(int)ImGuiCol.PlotLines]);
        _debugLineColorHovered = new Color(ImGui.GetStyle().Colors[(int)ImGuiCol.PlotLinesHovered]);

        base.Initialize();
    }

    private void InitializeViewport(bool resized = false)
    {
        if (_graphics == null)
            return;

        ApplyGraphicsSettings(resized);

        _viewportAdapter = new WindowViewportAdapter(Window, GraphicsDevice);
        _viewportAdapter.Reset();

        CenterWindow();

        _camera = new OrthographicCamera(_viewportAdapter);
    }

    private void ApplyGraphicsSettings(bool resized = false)
    {
        if (resized)
        {
            _graphics.PreferredBackBufferWidth = GraphicsDevice.Viewport.Width;
            _graphics.PreferredBackBufferHeight = GraphicsDevice.Viewport.Height;
        }
        else
        {
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
        }
        
        _graphics.IsFullScreen = false;
        _graphics.PreferMultiSampling = true;
        _graphics.SynchronizeWithVerticalRetrace = false;
        
        _graphics.ApplyChanges();
    }
    
    private void CenterWindow()
    {
        var windowCenter = new Point(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
        var screenCenter = new Point(GraphicsDevice.Adapter.CurrentDisplayMode.Width / 2, GraphicsDevice.Adapter.CurrentDisplayMode.Height / 2);
        Window.Position = screenCenter - windowCenter;
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    private TimeSpan _updateInterval = TimeSpan.FromSeconds(1);
    private TimeSpan _updateElapsed = TimeSpan.Zero;
    protected override void Update(GameTime gameTime)
    {
        MouseExtended.Update();
        KeyboardExtended.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.SetRenderTarget(_guiService.ViewportRenderTarget);
        
        var bgColor = new Color(ImGui.GetStyle().Colors[(int)ImGuiCol.WindowBg]);
        GraphicsDevice.Clear(bgColor);

        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred, 
            blendState: BlendState.NonPremultiplied,
            samplerState: SamplerState.PointClamp,
            transformMatrix: _camera.GetViewMatrix()
        );

        if (_gameStateManager.CurrentState != GameState.TurnBegin && _gameStateManager.CurrentState != GameState.GameOver)
        {
            var diceSprites = _gameStateManager.GetDiceSprites(DiceState.All);
            DrawDice(_spriteBatch, diceSprites);
        }

        if (DrawDebugLines)
        {
            var imguiViewportCenter = _guiService.ViewportRenderTarget.Bounds.Center;
            var imguiViewport = _guiService.ViewportRenderTarget.Bounds;
            _spriteBatch.DrawLine(
                point1: new Vector2(imguiViewportCenter.X, imguiViewport.Y), 
                point2: new Vector2(imguiViewportCenter.X, imguiViewport.Y + imguiViewport.Y), 
                color: _debugLineColor, 
                thickness: 1f, 
                layerDepth: 0f
            );
            _spriteBatch.DrawLine(
                point1: new Vector2(imguiViewport.X, imguiViewportCenter.Y), 
                point2: new Vector2(imguiViewport.X + imguiViewport.X, imguiViewportCenter.Y), 
                color: _debugLineColor, 
                thickness: 1f, 
                layerDepth: 0f
            );

            var viewport = _graphics.GraphicsDevice.Viewport;
            _spriteBatch.DrawLine(
                point1: new Vector2(viewport.Bounds.Center.X, viewport.Bounds.Top), 
                point2: new Vector2(viewport.Bounds.Center.X, viewport.Bounds.Bottom), 
                color: _debugLineColor, 
                thickness: 1f, 
                layerDepth: 0f
            );
            _spriteBatch.DrawLine(
                point1: new Vector2(viewport.Bounds.Left, viewport.Bounds.Center.Y), 
                point2: new Vector2(viewport.Bounds.Right, viewport.Bounds.Center.Y), 
                color: _debugLineColor, 
                thickness: 1f, 
                layerDepth: 0f
            );
        }

        _spriteBatch.End();

        GraphicsDevice.SetRenderTarget(null);

        _guiService.Draw(gameTime);

        base.Draw(gameTime);
    }

    public void ResetGame()
    {
        _gameStateManager.NewGame();
        _interfaceManager.ClearLog();
    }

    private void OnKeyPressed(object sender, KeyboardEventArgs args)
    {
        if (args.Key == Keys.Q || args.Key == Keys.Escape)
        {
            Exit();
        }

        if (args.Key == Keys.Space)
        {
            if (_gameStateManager.CurrentState == GameState.GameOver)
            {
                _gameStateManager.NewGame();
            }
            else
            {
                _gameStateManager.Roll();
            }
        }

        if (args.Key == Keys.D)
        {
            DrawDebugLines = !DrawDebugLines;
        }

        if (args.Key == Keys.R)
        {
            ResetGame();
        }

        if (args.Key == Keys.Enter)
        {
            if (_gameStateManager.CurrentState != GameState.RollActive)
                return;

            _gameStateManager.ScoreSelectedDice();
        }
    }
        
    private void OnMouseClicked(object sender, MouseEventArgs args)
    {
        if (args.Button != MouseButton.Left)
            return;

        if (_gameStateManager.CurrentState != GameState.RollActive)
            return;

        var mouseViewportPos = _guiService.GetMouseViewportPos();

        var diceSprites = _gameStateManager.GetDiceSprites(DiceState.Available, DiceState.Selected);
        for (var i = 0; i < diceSprites.Count; i++)
        {
            if (diceSprites[i].Bounds.Contains(mouseViewportPos))
            {
                diceSprites[i].Selected = !diceSprites[i].Selected;
                //diceSprites[i].Rotation = diceSprites[i].Selected ? MathHelper.ToRadians(Random.NextSingle(-20f, 20f)) : 0f;
                _gameStateManager.UpdateCurrentSelectionCombinations();
            }
        }
    }

    private RectangleF? PositionDiceSprites(IList<DiceSprite> diceSprites)
    {
        if (_gameStateManager.DiceManager == null)
            return null;
        if (diceSprites.Count == 0)
            return null;

        var viewport = GraphicsDevice.Viewport.Bounds;
        Vector2 diceMargins = 
            (diceSprites.Count == 1) ? 
                Vector2.Zero : 
                new Vector2(DiceSpriteService.SpriteOrigin.X * 0.25f, 0f);
        RectangleF diceBounds = new RectangleF(
            0, 
            0, 
            diceSprites.Count * DiceSpriteService.SpriteSize.X + ((diceSprites.Count - 1) * diceMargins.X), 
            DiceSpriteService.SpriteSize.Y
        );
        
        diceBounds.Position = new Vector2(viewport.Center.X - diceBounds.Center.X, viewport.Center.Y - diceBounds.Center.Y);

        for (var i = 0; i < diceSprites.Count; i++)
        {
            float x = (i == 0) ? 
                diceBounds.Left : 
                diceSprites[i - 1].Bounds.Right + diceMargins.X;
            
            float y = diceBounds.Top;
            if (diceSprites[i].Selected)
                y += DiceSpriteService.SpriteOrigin.Y;

            diceSprites[i].Position = new Vector2(x, y);

            diceSprites[i].Rotation = 0f;
        }
        
        if (diceSprites.Any(x => x.Selected)) 
            diceBounds.Height += DiceSpriteService.SpriteOrigin.Y;

        return diceBounds;
    }

    private Color _maskColor = new Color(0, 0, 0, 100);
    private Color _debugLineColorHovered;
    private Color _debugLineColor;

    private void DrawDice(SpriteBatch spriteBatch, IList<DiceSprite> diceSprites)
    {
        if (diceSprites.Count == 0)
            return;

        RectangleF? diceBounds = PositionDiceSprites(diceSprites);

        if (DrawDebugLines && diceBounds.HasValue)
        {
            _spriteBatch.DrawRectangle(
                rectangle: diceBounds.Value,
                color: _debugLineColor,
                thickness: 1f,
                layerDepth: 0f
            );
        }

        foreach (var diceSprite in diceSprites)
        {
            if (diceSprite.Dice.Value == 0)
                continue;

            var color = diceSprite.Available ? Color.White : _maskColor;
            var texture = _diceSpriteService.GetDiceSprite(diceSprite.Dice.Value);

            spriteBatch.Draw(
                texture: texture,
                position: diceSprite.Position + DiceSpriteService.SpriteOrigin,
                sourceRectangle: null,
                color: color,
                rotation: diceSprite.Rotation,
                origin: DiceSpriteService.SpriteOrigin,
                scale: 1f,
                effects: SpriteEffects.None,
                layerDepth: 0);

            if (DrawDebugLines)
            {
                Color lineColor = 
                    diceSprite.Bounds.Contains(_guiService.GetMouseViewportPos().ToVector2()) ? 
                        _debugLineColorHovered : 
                        _debugLineColor;
                
                _spriteBatch.DrawRectangle(
                    rectangle: diceSprite.Bounds,
                    color: lineColor,
                    thickness: 1f,
                    layerDepth: 0f
                );
            }

        }
    }

    
}