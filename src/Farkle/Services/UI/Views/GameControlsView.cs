using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Farkle.Rules.Scoring;
using Num = System.Numerics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Input;
using ImGuiNET;

namespace Farkle.Services.UI.Views;

public class GameControlsView : GuiViewBase
{
    public override string Name => "Game Controls";

    private GameStateManager _gameStateManager;
    private readonly GameMain _game;

    public GameControlsView(GameMain game) 
        : base(game)
    {
        _game = game;
    }

    public override void Initialize()
    {
        base.Initialize();

        _gameStateManager = _game.Services.GetService<GameStateManager>();
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(GameTime gameTime)
    {
        var viewport = ImGui.GetMainViewport();
        var viewportCenter = viewport.WorkSize * 0.5f;
        var viewportPadding = (viewport.Size.X - viewport.WorkSize.X) * 0.5f;
        var left = viewportCenter.X - (Size.X * 0.5f) - viewportPadding;
        var top = viewportCenter.Y + (Size.Y * 2);
        
        Size = new Num.Vector2(viewport.WorkSize.X * 0.5f, 100f);
        Position = new Num.Vector2(left, top);
        DockId = GuiService.WindowDockId;

        ImGui.SetNextWindowSize(Size, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowPos(Position, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowDockID(DockId, ImGuiCond.FirstUseEver);

        Num.Vector2 buttonSize = new Num.Vector2(ImGui.GetContentRegionAvail().X / 3f, 30f);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, ImGui.GetStyle().WindowPadding * 2f);

        if (ImGui.Begin(Name, ref IsVisible, ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoBackground))
        {
            WindowId = ImGui.GetItemID();

            ImGui.Text("Turn:"); 
            ImGui.SameLine(); 
            ImGui.Text(_gameStateManager.Turn.ToString());
            
            ImGui.SameLine(ImGui.GetContentRegionAvail().X * 0.5f);

            ImGui.Text("Game state:");
            ImGui.SameLine();
            ImGui.Text(_gameStateManager.CurrentState.ToString());

            ImGui.Spacing();

            DrawButtonTable();

            ImGui.End();
        }

        ImGui.PopStyleVar();
    }

    private void DrawButtonTable()
    {
        int buttonCount = 2;

        if (ImGui.BeginTable("##Buttons", buttonCount, ImGuiTableFlags.SizingStretchSame))
        {
            if (_gameStateManager.CurrentState == GameState.GameOver)
            {
                ImGui.TableSetupColumn("##NewGame", ImGuiTableColumnFlags.WidthStretch);
            }
            else
            {
                ImGui.TableSetupColumn("##RollScore", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableSetupColumn("##EndTurn", ImGuiTableColumnFlags.WidthStretch);
            }

            ImGui.TableNextRow();

            DrawButtonColumns();
        }
    }

    private void DrawButtonColumns()
    {
        ImGui.TableNextColumn();

        if (_gameStateManager.CurrentState == GameState.GameOver)
        {
            if (ImGui.Button("New Game", new Num.Vector2(ImGui.GetContentRegionAvail().X, -1f)))
            {
                _gameStateManager.NewGame();
            }

            ImGui.EndTable();
            return;
        }

        if (_gameStateManager.CurrentState == GameState.TurnBegin || _gameStateManager.CurrentState == GameState.RollComplete)
        {
            var rollButtonEnabled = _gameStateManager.CurrentState == GameState.TurnBegin || _gameStateManager.CurrentState == GameState.RollComplete;
            if (!rollButtonEnabled)
                ImGui.BeginDisabled();

            if (ImGui.Button("Roll", new Num.Vector2(ImGui.GetContentRegionAvail().X, -1f)))
            {
                _gameStateManager.Roll();
            }

            if (!rollButtonEnabled)
                ImGui.EndDisabled();
        }
        else
        {
            var scoreButtonEnabled = 
                _gameStateManager.CurrentState == GameState.RollActive && 
                _gameStateManager.HasSelectedDice && 
                _gameStateManager.SelectedPossibleScoredSet != ScoredSet.None;

            if (!scoreButtonEnabled)
                ImGui.BeginDisabled();

            if (ImGui.Button("Score Selected Dice", new Num.Vector2(ImGui.GetContentRegionAvail().X, -1f)))
            {
                _gameStateManager.ScoreSelectedDice();
            }

            if (!scoreButtonEnabled)
                ImGui.EndDisabled();
        }

        ImGui.TableNextColumn();

        {
            var endTurnButtonEnabled = _gameStateManager.CurrentState == GameState.RollActive || _gameStateManager.CurrentState == GameState.RollComplete;

            if (!endTurnButtonEnabled)
                ImGui.BeginDisabled();

            if (ImGui.Button("End Turn", new Num.Vector2(ImGui.GetContentRegionAvail().X, -1f)))
            {
                _gameStateManager.EndTurn();
                _gameStateManager.StartTurn();
            }

            if (!endTurnButtonEnabled)
                ImGui.EndDisabled();
        }

        ImGui.EndTable();
    }

}