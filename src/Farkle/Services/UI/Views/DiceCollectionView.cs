using System;
using System.Collections.Generic;
using System.Linq;
using Num = System.Numerics;
using Microsoft.Xna.Framework;
using ImGuiNET;
using Farkle.Rules.DiceTypes;

namespace Farkle.Services.UI.Views;

public class DiceCollectionView : GuiViewBase
{
    private bool _diceCollectionOpen = true;
    private GameStateManager _gameStateManager;
    private DiceSpriteService _diceSpriteService;
    public override string Name => "Dice Collection";

    public DiceCollectionView(GameMain game)
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

    public override unsafe void Draw(GameTime gameTime)
    {
        var viewport = ImGui.GetMainViewport();

        var windowSize = new Num.Vector2(viewport.WorkSize.X * 0.5f, viewport.WorkSize.Y * 0.2f);
        var windowPos = new Num.Vector2(viewport.WorkPos.X, viewport.WorkSize.Y - windowSize.Y);

        ImGui.SetNextWindowSize(windowSize, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowPos(windowPos, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowDockID(GuiService.WindowDockId, ImGuiCond.FirstUseEver);

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, ImGui.GetStyle().WindowPadding * 1.5f);

        if (ImGui.Begin("Dice Collection", ref _diceCollectionOpen, ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoCollapse))
        {
            WindowID = ImGui.GetItemID();

            var diceTypes = _gameStateManager.GetDiceTypes();
            var comboOptions = diceTypes.Keys.ToList();
            
            ImGui.Text("Active Dice");
            ImGui.Separator();
            
            List<(DiceBase DiceBase, DiceSprite DiceSprite)> dice = 
                _gameStateManager.GetDiceSprites(DiceState.All).Select(x => (x.Dice, x)).ToList();

            for (int i = 0; i < dice.Count; i++)
            {
                ImGui.Text($"{i + 1}.");
                ImGui.SameLine(0f, ImGui.GetStyle().ItemSpacing.X);
                
                int selectedIndex = comboOptions.IndexOf(dice[i].DiceBase.Name);
                ImGui.SetNextItemWidth(200f);

                var popupBg = ImGui.GetStyle().Colors[(int)ImGuiCol.PopupBg];
                popupBg.W = 1f;

                ImGui.PushStyleColor(ImGuiCol.PopupBg, popupBg);

                if (ImGui.BeginCombo($"##Die{i}", comboOptions[selectedIndex], ImGuiComboFlags.None))
                {
                    for (int j = 0; j < comboOptions.Count; j++)
                    {
                        bool isSelected = selectedIndex == j;
                        if (ImGui.Selectable(comboOptions[j], isSelected, ImGuiSelectableFlags.None))
                        {
                            selectedIndex = j;
                            dice[i] = _gameStateManager.DiceManager.ChangeDiceType(dice[i].DiceBase, diceTypes[comboOptions[selectedIndex]]);
                        }

                        if (isSelected) 
                            ImGui.SetItemDefaultFocus();
                    }

                    ImGui.EndCombo();
                }

                ImGui.PopStyleColor();

                ImGui.SameLine(0f, ImGui.GetStyle().ItemSpacing.X * 2f);
                ImGui.Text(dice[i].DiceBase.Description);
            }

            ImGui.End();
        }

        ImGui.PopStyleVar();
    }
}