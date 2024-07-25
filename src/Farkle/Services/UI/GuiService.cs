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

    public GuiService(GameMain game)
    {
        _game = game;
    }

    private IntPtr _renderTexturePtr;
    private Texture2D _renderTexture;
    private RenderTarget2D _renderTarget;
    public IntPtr GetRenderTexturePtr() => _renderTexturePtr;

    private GameAreaView _gameAreaView;
    public GameAreaView GameAreaView => _gameAreaView;

    public override void Initialize()
    {
        _imGuiRenderer = _game.Services.GetService<ImGuiRenderer>();
        _gameStateManager = _game.Services.GetService<GameStateManager>();
        _diceSpriteService = _game.Services.GetService<DiceSpriteService>();
        _inputManager = _game.Services.GetService<InputManager>();
        
        //var theme = new SemiDarkGuiTheme();
        //theme.ApplyTheme();

        _imGuiRenderer.RebuildFontAtlas();

        _views.Add(new ScorecardView(_game));
        _views.Add(new DiceCollectionView(_game));
        
        _gameAreaView = new GameAreaView(_game);
        _views.Add(_gameAreaView);
        
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

        foreach (GuiViewBase view in _views)
        {
            view.Draw(gameTime);
        }

        if (DebuggingWindows.Metrics) ImGui.ShowMetricsWindow(ref DebuggingWindows.Metrics);
        if (DebuggingWindows.DebugLog) ImGui.ShowDebugLogWindow(ref DebuggingWindows.DebugLog);
        if (DebuggingWindows.StackTool) ImGui.ShowIDStackToolWindow(ref DebuggingWindows.StackTool);
        if (DebuggingWindows.StyleEditor) ImGui.ShowStyleEditor(ImGui.GetStyle());
        if (DebuggingWindows.StyleSelector) ImGui.ShowStyleSelector("Style Selector");
        if (DebuggingWindows.FontSelector) ImGui.ShowFontSelector("Font Selector");
        if (DebuggingWindows.ImGuiDemo) ImGui.ShowDemoWindow(ref DebuggingWindows.ImGuiDemo);

        _imGuiRenderer.AfterLayout();
    }


    private void CreateMainWindow(GameTime gameTime)
    {
        var viewport = ImGui.GetMainViewport();

        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);
        ImGui.SetNextWindowViewport(viewport.ID);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Num.Vector2.Zero);

        ImGuiWindowFlags windowFlags =
            ImGuiWindowFlags.NoDocking |
            ImGuiWindowFlags.NoTitleBar |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoDecoration | 
            ImGuiWindowFlags.NoBackground;
        
        windowFlags |= 
            //ImGuiWindowFlags.NoBringToFrontOnFocus | 
            ImGuiWindowFlags.NoFocusOnAppearing; // | 
            //ImGuiWindowFlags.NoNav | 
            //ImGuiWindowFlags.NoNavFocus; 
     
        ImGui.Begin("MainWindow", windowFlags);
            
        ImGui.PopStyleVar(3);

        _windowDockId = ImGui.GetID("MainDockspace");

        ImGuiDockNodeFlags dockNodeFlags = ImGuiDockNodeFlags.PassthruCentralNode; // | ImGuiDockNodeFlags.PassthruCentralNode; 

        ImGui.DockSpace(_windowDockId, Num.Vector2.Zero, dockNodeFlags);

        CreateMainMenuBar();

        ImGui.End();
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