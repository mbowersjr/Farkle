using Num = System.Numerics;
using MonoGame.Extended;

namespace Farkle.Services.UI.Views;

public abstract class GuiViewBase : SimpleDrawableGameComponent
{
    public abstract string Name { get; }
    public uint WindowId;
    public uint DockId;
    public Num.Vector2 Size;
    public Num.Vector2 Position;
    public bool IsVisible;

    protected GameMain Game { get; private set; }
    protected GuiService GuiService { get; private set; }

    protected GuiViewBase(GameMain game)
    {
        Game = game;
    }

    public override void Initialize()
    {
        base.Initialize();
        GuiService = Game.Services.GetService<GuiService>();
    }
}