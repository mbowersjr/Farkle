using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Num = System.Numerics;
using Microsoft.Xna.Framework;
using ImGuiNET;

namespace Farkle.Services.UI.Views;

public class ScorecardView : GuiViewBase
{
    private bool _scorecardOpen = true;
    private GameStateManager _gameStateManager;
    private DiceSpriteService _diceSpriteService;

    public override string Name => "Scorecard";

    public ScorecardView(GameMain game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        base.Initialize();

        _gameStateManager = Game.Services.GetService<GameStateManager>();
        _diceSpriteService = Game.Services.GetService<DiceSpriteService>();
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime)
    {
        var viewport = ImGui.GetMainViewport();

        var windowSize = new Num.Vector2(viewport.WorkSize.X * 0.2f, viewport.WorkSize.Y);
        var windowPos = new Num.Vector2(viewport.WorkSize.X - windowSize.X, viewport.WorkPos.Y);
        
        ImGui.SetNextWindowSize(windowSize, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowPos(windowPos, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowDockID(GuiService.WindowDockId, ImGuiCond.FirstUseEver);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, ImGui.GetStyle().WindowPadding * 1.5f);
        
        if (ImGui.Begin("Scorecard", ref _scorecardOpen, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse))
        {
            WindowID = ImGui.GetItemID();

            ImGui.Text("Scorecard");
            ImGui.Separator();

            
            if (ImGui.BeginTable("Scores", 2, ImGuiTableFlags.None))
            {
                ImGui.TableSetupColumn("Dice", ImGuiTableColumnFlags.WidthFixed, ImGui.GetContentRegionAvail().X * 0.75f);
                ImGui.TableSetupColumn("Score", ImGuiTableColumnFlags.WidthFixed);
                ImGui.TableHeadersRow();

                for (int i = 0; i < _gameStateManager.ScoredSets.Count; i++)
                {
                    ImGui.TableNextRow(ImGuiTableRowFlags.None);

                    ImGui.TableSetColumnIndex(0);
                    float diceSize = ImGui.GetContentRegionAvail().X / 6f - (25f);

                    var set = _gameStateManager.ScoredSets[i];
                    for (int j = 0; j < set.Dice.Count; j++)
                    {
                        ImGui.Image(_diceSpriteService.GetDiceImagePtr(set.Dice[j].Value), new Num.Vector2(diceSize, diceSize));
                        if (j < set.Dice.Count)
                            ImGui.SameLine(0f, 5f);
                    }

                    ImGui.TableSetColumnIndex(1);
                    ImGui.Text(set.Score.ToString());
                }

                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
                ImGui.TableSetColumnIndex(1);
                ImGui.Text(_gameStateManager.ScoredSets.Sum(x => x.Score).ToString());

                ImGui.EndTable();
            }

            ImGui.End();
        }

        ImGui.PopStyleVar();
    }
}