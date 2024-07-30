using System;
using System.Collections.Generic;
using System.Linq;
using Num = System.Numerics;
using Microsoft.Xna.Framework;
using ImGuiNET;
using Farkle.Rules.DiceTypes;
using System.Reflection;
using System.Text;

namespace Farkle.Services.UI.Views;

public class DiceCollectionView : GuiViewBase
{
    public override string Name => "Dice Collection";

    private GameStateManager _gameStateManager;
    private DiceSpriteService _diceSpriteService;

    public DiceCollectionView(GameMain game)
        : base(game)
    {
    }

    public override void Initialize()
    {
        base.Initialize();

        _gameStateManager = Game.Services.GetService<GameStateManager>();
        _diceSpriteService = Game.Services.GetService<DiceSpriteService>();

        _diceTypes = _gameStateManager.GetDiceTypes();
        _comboOptions = _diceTypes.Keys.ToList();
    }

    public override void Update(GameTime gameTime)
    {
    }

    private Dictionary<string, Type> _diceTypes;
    private List<string> _comboOptions;

    public override void Draw(GameTime gameTime)
    {
        var viewport = ImGui.GetMainViewport();

        Size = new Num.Vector2(viewport.WorkSize.X * 0.5f, viewport.WorkSize.Y * 0.2f);
        Position = new Num.Vector2(viewport.WorkPos.X, viewport.WorkSize.Y - Size.Y);
        DockId = GuiService.WindowDockId;

        ImGui.SetNextWindowSize(Size, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowPos(Position, ImGuiCond.FirstUseEver);
        ImGui.SetNextWindowDockID(DockId, ImGuiCond.FirstUseEver);
        
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, ImGui.GetStyle().WindowPadding * 2f);

        if (ImGui.Begin(Name, ref IsVisible, ImGuiWindowFlags.None))
        {
            WindowId = ImGui.GetItemID();

            ImGui.SeparatorText("Active Dice");

            List<(DiceBase DiceBase, DiceSprite DiceSprite)> dice =
                _gameStateManager.GetDiceSprites(DiceState.All)
                    .Select(x => (x.Dice, x)).ToList();

            for (int i = 0; i < dice.Count; i++)
            {
                DrawDiceComponent(i, dice[i]);
            }

            ImGui.End();
        }

        ImGui.PopStyleVar();
    }

    private void DrawDiceComponent(int index, (DiceBase DiceBase, DiceSprite DiceSprite) dice)
    {
        ImGui.AlignTextToFramePadding();
        ImGui.Text($"{index + 1}.");
        ImGui.SameLine();
                
        ImGui.SetNextItemWidth(200f);

        DrawDiceTypeCombo(index, dice);
    }

    private void DrawDiceTypeCombo(int index, (DiceBase DiceBase, DiceSprite DiceSprite) dice)
    {
        int selectedIndex = _comboOptions.IndexOf(dice.DiceBase.Name);
        string currentDiceTypeName = _comboOptions[selectedIndex];

        var popupBg = ImGui.GetStyle().Colors[(int)ImGuiCol.PopupBg];
        popupBg.W = 1f;
        ImGui.PushStyleColor(ImGuiCol.PopupBg, popupBg);

        if (ImGui.BeginCombo($"##Die{index}", currentDiceTypeName, ImGuiComboFlags.None))
        {
            for (int i = 0; i < _comboOptions.Count; i++)
            {
                bool isSelected = selectedIndex == i;
                if (ImGui.Selectable(_comboOptions[i], isSelected, ImGuiSelectableFlags.None))
                {
                    selectedIndex = i;

                    string selectedDiceTypeName = _comboOptions[selectedIndex];
                    Type newDiceType = _diceTypes[selectedDiceTypeName];

                    dice = _gameStateManager.DiceManager.ChangeDiceType(dice.DiceBase, newDiceType);
                }

                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }

            ImGui.EndCombo();
        }

        if (!ImGui.IsItemToggledOpen())
        {
            if (ImGui.BeginItemTooltip())
            {
                ImGui.SeparatorText("Description");
                ImGui.Text(dice.DiceBase.Description);
                ImGui.Spacing();
                ImGui.SeparatorText("Probability");
                for (int i = 0; i < dice.DiceBase.Weights.Length; i++)
                {
                    ImGui.Text($"{i+1}: {dice.DiceBase.Weights[i].Weight*100f:N2}%%");
                }

                ImGui.EndTooltip();
            }
        }

        ImGui.PopStyleColor();
    }
}