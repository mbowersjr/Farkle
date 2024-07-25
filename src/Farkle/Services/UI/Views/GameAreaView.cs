using System;
using Num = System.Numerics;
using ImGuiNET;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Farkle.Services.UI.Views;

public class GameAreaView : GuiViewBase
{
    public override string Name => "Game Area";
    public Num.Vector2 RenderAreaSize { get; set; }
    public IntPtr RenderTexturePtr { get; set; }
    public RenderTarget2D RenderTarget { get; set; }
    public Texture2D RenderTexture { get; set; }

    public GameAreaView(GameMain game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        base.Initialize();

        ResizeRenderTexture(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime)
    {
        var windowFlags =
            ImGuiWindowFlags.NoMove |
            ImGuiWindowFlags.NoCollapse |
            ImGuiWindowFlags.NoDecoration |
            ImGuiWindowFlags.NoResize |
            ImGuiWindowFlags.NoScrollbar |
            ImGuiWindowFlags.NoTitleBar | 
            ImGuiWindowFlags.NoFocusOnAppearing |
            ImGuiWindowFlags.NoNavFocus |
            ImGuiWindowFlags.NoBringToFrontOnFocus;

        var viewport = ImGui.GetMainViewport();
        var windowSize = new Num.Vector2(100f, 100f);
        var windowPos = new Num.Vector2(viewport.WorkPos.X, viewport.WorkPos.Y);

        ImGui.SetNextWindowSize(windowSize, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowPos(windowPos, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowDockID(GuiService.WindowDockId, ImGuiCond.FirstUseEver);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 0f);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new System.Numerics.Vector2(0f));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, new System.Numerics.Vector2(0f));

        if (ImGui.Begin("GameArea", windowFlags))
        {
            WindowID = ImGui.GetItemID();
            
            var contentRegionAvail = ImGui.GetContentRegionAvail();
            ResizeRenderTexture(contentRegionAvail.X, contentRegionAvail.Y);

            UpdateRenderTexture();

            ImGui.Image(RenderTexturePtr, RenderAreaSize);

            ImGui.End();
        }

        ImGui.PopStyleVar(3);
    }

    private void UpdateRenderTexture()
    {
        if (RenderTarget == null || RenderTexture == null)
            return;

        Color[] textureData = new Color[RenderTarget.Width * RenderTarget.Height];
        RenderTarget.GetData(textureData);
        RenderTexture.SetData(textureData);
    }

    private void ResizeRenderTexture(float width, float height)
    {
        if (RenderTexture != null && RenderTexture.Bounds.Size.X == (int)width && RenderTexture.Bounds.Size.Y == (int)height)
            return;

        RenderTarget = new RenderTarget2D(Game.GraphicsDevice, (int)width, (int)height);
        RenderTexture = new Texture2D(Game.GraphicsDevice, (int)width, (int)height);
        RenderAreaSize = new Num.Vector2(width, height);

        if (RenderTexturePtr != IntPtr.Zero)
            GuiService.ImGuiRenderer.UnbindTexture(RenderTexturePtr);
        
        RenderTexturePtr = GuiService.ImGuiRenderer.BindTexture(RenderTexture);
    }
}