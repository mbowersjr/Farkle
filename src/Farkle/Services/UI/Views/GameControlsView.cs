using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;
using Microsoft.Xna.Framework;

namespace Farkle.Services.UI.Views;

public class GameControlsView : GuiViewBase
{
    public override string Name => "Game Controls";

    public GameControlsView(GameMain game) : base(game)
    {
    }

    public override void Update(GameTime gameTime)
    {
        throw new NotImplementedException();
    }

    public override void Draw(GameTime gameTime)
    {
        throw new NotImplementedException();
    }

}