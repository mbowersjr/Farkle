using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farkle.Rules.DiceTypes;
using Farkle.Services.UI.Themes;
using Farkle.Services.UI.Views;
using Num = System.Numerics;
using Microsoft.Xna.Framework;
using ImGuiNET;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Farkle.Services.UI;

public class GuiService : SimpleDrawableGameComponent
{
    private readonly GameMain _game;
    public ImGuiRenderer ImGuiRenderer => _imGuiRenderer;
    private ImGuiRenderer _imGuiRenderer;
    private InputManager _inputManager;
    private GameStateManager _gameStateManager;
    private DiceSpriteService _diceSpriteService;

    private uint _windowDockId;
    public uint WindowDockId => _windowDockId;

    private List<GuiViewBase> _views = new List<GuiViewBase>();

    
    private Num.Vector2 _renderAreaSize;
    private IntPtr _renderTexturePtr;
    private Texture2D _renderTexture;
    private RenderTarget2D _renderTarget;
    public RenderTarget2D ViewportRenderTarget => _renderTarget;

    public GuiService(GameMain game)
    {
        _game = game;
    }

    public override void Initialize()
    {
        _imGuiRenderer = _game.Services.GetService<ImGuiRenderer>();
        _gameStateManager = _game.Services.GetService<GameStateManager>();
        _diceSpriteService = _game.Services.GetService<DiceSpriteService>();
        _inputManager = _game.Services.GetService<InputManager>();
        
        //var theme = new SemiDarkGuiTheme();
        //theme.ApplyTheme();

        _imGuiRenderer.RebuildFontAtlas();

        ResizeRenderTexture(_game.GraphicsDevice.Viewport.Width, _game.GraphicsDevice.Viewport.Height);

        _views.Add(new ScorecardView(_game));
        _views.Add(new DiceCollectionView(_game));
        
        foreach (GuiViewBase view in _views)
        {
            view.Initialize();
        }

        base.Initialize();
    }

    protected override void LoadContent()
    {
        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (var view in _views)
        {
            view.Update(gameTime);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        var io = ImGui.GetIO();
        io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
        // io.ConfigFlags |= ImGuiConfigFlags.DpiEnableScaleFonts | ImGuiConfigFlags.DpiEnableScaleViewports;

        _imGuiRenderer.BeforeLayout(gameTime);
        
        _inputManager.GuiWantCaptureKeyboard = io.WantCaptureKeyboard;
        _inputManager.GuiWantCaptureMouse = io.WantCaptureMouse;
        _inputManager.GuiWantCaptureMouseUnlessPopupClose = io.WantCaptureMouseUnlessPopupClose;
        _inputManager.GuiWantTextInput = io.WantTextInput;

        CreateMainWindow(gameTime);

        if (DebuggingWindows.Metrics) ImGui.ShowMetricsWindow(ref DebuggingWindows.Metrics);
        if (DebuggingWindows.DebugLog) ImGui.ShowDebugLogWindow(ref DebuggingWindows.DebugLog);
        if (DebuggingWindows.StackTool) ImGui.ShowIDStackToolWindow(ref DebuggingWindows.StackTool);
        if (DebuggingWindows.StyleEditor) ImGui.ShowStyleEditor(ImGui.GetStyle());
        if (DebuggingWindows.StyleSelector) ImGui.ShowStyleSelector("Style Selector");
        if (DebuggingWindows.FontSelector) ImGui.ShowFontSelector("Font Selector");
        if (DebuggingWindows.ImGuiDemo) ImGui.ShowDemoWindow(ref DebuggingWindows.ImGuiDemo);

        _imGuiRenderer.AfterLayout();
    }

    private bool _dockspaceOpen = true;
    private bool _fullscreen = true;
    
    private void CreateMainWindow(GameTime gameTime)
    {
        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.NoDocking;

        if (_fullscreen)
        {
            var viewport = ImGui.GetMainViewport();

            ImGui.SetNextWindowPos(viewport.WorkPos);
            ImGui.SetNextWindowSize(viewport.WorkSize);
            ImGui.SetNextWindowViewport(viewport.ID);    
            ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
            ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);

            windowFlags |= ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove;
            windowFlags |= ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoNavFocus;
        }
     
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Num.Vector2(0f, 0f));
        ImGui.Begin("MainWindow", ref _dockspaceOpen, windowFlags);
        ImGui.PopStyleVar();
        
        if (_fullscreen)
            ImGui.PopStyleVar(2);

        var io = ImGui.GetIO();
        var style = ImGui.GetStyle();
        float minWinSizeX = style.WindowMinSize.X;
        style.WindowMinSize.X = 370f;

        _windowDockId = ImGui.GetID("MainDockspace");
        ImGuiDockNodeFlags dockNodeFlags = ImGuiDockNodeFlags.PassthruCentralNode; 
        ImGui.DockSpace(_windowDockId, Num.Vector2.Zero, dockNodeFlags);

        style.WindowMinSize.X = minWinSizeX;
        
        CreateMainMenuBar();
        
        foreach (GuiViewBase view in _views)
        {
            view.Draw(gameTime);
        }

        CreateViewport();

        ImGui.End();
    }

    private bool _viewportFocused = false;
    private bool _viewportHovered = false;
    private RectangleF _viewportBounds;
    private Num.Vector2 _viewportSize;
    private bool _viewportOpen = true;
    public Num.Vector2 ViewportMousePos { get; set; }

    private void CreateViewport()
    {
        var windowFlags = ImGuiWindowFlags.None;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Num.Vector2.Zero);

        if (ImGui.Begin("Viewport", ref _viewportOpen, windowFlags))
        {
            var viewportMinRegion = ImGui.GetWindowContentRegionMin();
            var viewportMaxRegion = ImGui.GetWindowContentRegionMax();
            var viewportOffset = ImGui.GetWindowPos();

            _viewportBounds = new RectangleF(
                x: viewportMinRegion.X + viewportOffset.X,
                y: viewportMinRegion.Y + viewportOffset.Y,
                width: viewportMaxRegion.X - viewportMinRegion.X,
                height: viewportMaxRegion.Y - viewportMinRegion.Y
            );

            _viewportFocused = ImGui.IsWindowFocused();
            _viewportHovered = ImGui.IsWindowHovered();

            _viewportSize = ImGui.GetContentRegionAvail();

            //if (ImGui.IsWindowHovered() || ImGui.IsWindowFocused())
            //{
            //    var viewportPos = ImGui.GetCursorPos();
            //    var mousePos = ImGui.GetMousePos();
            //    if 
            //}

            ResizeRenderTexture(_viewportSize.X, _viewportSize.Y);
            UpdateRenderTexture();

            ImGui.Image(_renderTexturePtr, _viewportSize);

            ImGui.End();
        }

        ImGui.PopStyleVar();
    }

    public Num.Vector2 GetMouseViewportPos()
    {
        return ImGui.GetMousePos() - ImGui.GetMainViewport().WorkPos;
    }

    private void CreateMainMenuBar()
    {
        if (ImGui.BeginMainMenuBar())
        {
            if (ImGui.BeginMenu("Farkle"))
            {
                if (ImGui.MenuItem("New Game"))
                {
                    _game.ResetGame();
                }
                
                if (ImGui.MenuItem("Reset"))
                {
                    _game.ResetGame();
                }

                ImGui.Separator();

                if (ImGui.MenuItem("Exit"))
                {
                    _game.Exit();
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("Options"))
            {
                if (ImGui.MenuItem("Draw Debug Lines", null, GameMain.DrawDebugLines))
                {
                    GameMain.DrawDebugLines = !GameMain.DrawDebugLines;
                }

                ImGui.EndMenu();
            }

            if (ImGui.BeginMenu("ImGui Tools"))
            {
                ImGui.MenuItem("ImGui Metrics", null, ref DebuggingWindows.Metrics, true);
                ImGui.MenuItem("Stack Tool", null, ref DebuggingWindows.StackTool, true);
                ImGui.MenuItem("Debug Log", null, ref DebuggingWindows.DebugLog, true);
                ImGui.MenuItem("Style Editor", null, ref DebuggingWindows.StyleEditor, true);
                ImGui.MenuItem("Style Selector", null, ref DebuggingWindows.StyleSelector, true);
                ImGui.MenuItem("Font Selector", null, ref DebuggingWindows.FontSelector, true);
                ImGui.MenuItem("ImGui Demo", null, ref DebuggingWindows.ImGuiDemo, true);

                ImGui.EndMenu();
            }

            ImGui.EndMainMenuBar();
        }
    }
    
    private void UpdateRenderTexture()
    {
        if (_renderTarget == null || _renderTexture == null)
            return;

        Color[] textureData = new Color[_renderTarget.Width * _renderTarget.Height];
        _renderTarget.GetData(textureData);
        _renderTexture.SetData(textureData);
    }

    private void ResizeRenderTexture(float width, float height)
    {
        if (_renderTexture != null && _renderTexture.Bounds.Size.X == (int)width && _renderTexture.Bounds.Size.Y == (int)height)
            return;

        _renderTarget = new RenderTarget2D(_game.GraphicsDevice, (int)width, (int)height);
        _renderTexture = new Texture2D(_game.GraphicsDevice, (int)width, (int)height);
        _renderAreaSize = new Num.Vector2(width, height);

        if (_renderTexturePtr != IntPtr.Zero)
            _imGuiRenderer.UnbindTexture(_renderTexturePtr);
        
        _renderTexturePtr = _imGuiRenderer.BindTexture(_renderTexture);
    }

    private static class DebuggingWindows
    {
        public static bool Metrics = false;
        public static bool StackTool = false;
        public static bool DebugLog = false;
        public static bool StyleEditor = false;
        public static bool StyleSelector = false;
        public static bool FontSelector = false;
        public static bool ImGuiDemo = false;
    }
}