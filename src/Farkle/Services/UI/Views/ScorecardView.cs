using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farkle.Rules.Scoring;
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
        //ImGui.SetNextWindowDockID(GuiService.WindowDockId, ImGuiCond.FirstUseEver);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, ImGui.GetStyle().WindowPadding * 2f);

        if (ImGui.Begin("Scorecard", ref _scorecardOpen))
        {
            WindowID = ImGui.GetItemID();

            ImGui.SeparatorText("Scorecard");

            if (ImGui.BeginTable("Scores", 2, ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInner))
            {
                ImGui.TableSetupColumn("Dice", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Score", ImGuiTableColumnFlags.WidthFixed, 100f);
                ImGui.TableHeadersRow();

                DrawScoredSets();

                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
                ImGui.TableSetColumnIndex(1);
                ImGui.Text(_gameStateManager.ScoredSets.Sum(x => x.Score).ToString());

                ImGui.EndTable();
            }

            ImGui.End();
        }

        ImGui.PopStyleVar();
    }

    private void DrawScoredSets()
    {
        for (int i = 0; i < _gameStateManager.ScoredSets.Count; i++)
        {
            ImGui.TableNextRow(ImGuiTableRowFlags.None);

            ImGui.TableSetColumnIndex(0);

            var set = _gameStateManager.ScoredSets[i];
            DrawScoredSetDice(set);

            ImGui.TableSetColumnIndex(1);
            ImGui.Text(set.Score.ToString());
        }
    }

    private void DrawScoredSetDice(ScoredSet set)
    {
        float diceImageSeparation = 5f;
        float diceImageSize = (ImGui.GetContentRegionAvail().X / 6f) - (diceImageSeparation * (set.Dice.Count - 1));

        for (int i = 0; i < set.Dice.Count; i++)
        {
            var diceImagePtr = _diceSpriteService.GetDiceImagePtr(set.Dice[i].Value);
            ImGui.Image(diceImagePtr, new Num.Vector2(diceImageSize, diceImageSize));

            if (i < set.Dice.Count)
            {
                ImGui.SameLine(0f, diceImageSeparation);
            }
        }
    }
}