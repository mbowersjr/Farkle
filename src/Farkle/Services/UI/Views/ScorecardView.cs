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
    public override string Name => "Scorecard";

    private GameStateManager _gameStateManager;
    private DiceSpriteService _diceSpriteService;

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

        Size = new Num.Vector2(viewport.WorkSize.X * 0.2f, viewport.WorkSize.Y);
        Position = new Num.Vector2(viewport.WorkSize.X - Size.X, viewport.WorkPos.Y);

        ImGui.SetNextWindowSize(Size, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowPos(Position, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowDockID(DockId, ImGuiCond.FirstUseEver);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, ImGui.GetStyle().WindowPadding * 2f);

        ImGuiWindowFlags windowFlags = ImGuiWindowFlags.None;

        if (ImGui.Begin(Name, ref IsVisible, windowFlags))
        {
            WindowId = ImGui.GetItemID();

            ImGui.SeparatorText("Scorecard");

            if (ImGui.BeginTable("Scores", 2, ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersInner))
            {
                ImGui.TableSetupColumn("Dice", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("Score", ImGuiTableColumnFlags.WidthFixed, 100f);
                ImGui.TableHeadersRow();

                DrawScoredSets();

                ImGui.TableNextRow(ImGuiTableRowFlags.Headers);
                ImGui.TableSetColumnIndex(1);
                int totalScore = _gameStateManager.GetTotalScore();
                ImGui.Text($"{totalScore}");

                ImGui.EndTable();
            }

            ImGui.End();
        }

        ImGui.PopStyleVar();
    }

    private void DrawScoredSets()
    {
        var scoredSets = _gameStateManager.GetScoredSets();
        for (int i = 0; i < scoredSets.Count; i++)
        {
            ImGui.TableNextRow();

            ImGui.TableSetColumnIndex(0);

            var set = scoredSets[i];
            DrawScoredSetDice(set);

            ImGui.TableSetColumnIndex(1);
            
            ImGui.PushStyleVar(ImGuiStyleVar.SelectableTextAlign, new Num.Vector2(0.5f, 0.5f));
            ImGui.PushStyleVar(ImGuiStyleVar.DisabledAlpha, 1f);
            
            ImGui.Selectable(set.Score.ToString(), false, ImGuiSelectableFlags.Disabled, ImGui.GetItemRectSize());
            
            ImGui.PopStyleVar(2);
        }
    }

    private void DrawScoredSetDice(ScoredSet set)
    {
        float diceImageSizeX = ImGui.GetContentRegionAvail().X / 6f - (ImGui.GetStyle().ItemSpacing.X);

        for (int i = 0; i < set.Dice.Count; i++)
        {
            var diceImagePtr = _diceSpriteService.GetDiceImagePtr(set.Dice[i].Value);
            ImGui.Image(diceImagePtr, new Num.Vector2(diceImageSizeX, diceImageSizeX));

            if (i < set.Dice.Count)
                ImGui.SameLine();
        }
    }
}