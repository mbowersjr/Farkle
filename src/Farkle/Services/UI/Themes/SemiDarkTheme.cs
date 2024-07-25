using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImGuiNET;

namespace Farkle.Services.UI.Themes;

public class SemiDarkGuiTheme : GuiThemeBase
{
    public override ImGuiStylePtr ApplyTheme()
    {
        var style = ImGui.GetStyle();
        var colors = style.Colors;

        colors[(int)ImGuiCol.Text]                   = new (0.73f, 0.73f, 0.73f, 1.00f);
        colors[(int)ImGuiCol.TextDisabled]           = new (0.35f, 0.35f, 0.35f, 1.00f);
        colors[(int)ImGuiCol.WindowBg]               = new (0.14f, 0.14f, 0.15f, 0.86f);
        colors[(int)ImGuiCol.ChildBg]                = new (0.14f, 0.14f, 0.15f, 0.00f);
        colors[(int)ImGuiCol.PopupBg]                = new (0.14f, 0.14f, 0.15f, 0.86f);
        colors[(int)ImGuiCol.Border]                 = new (0.33f, 0.33f, 0.33f, 0.46f);
        colors[(int)ImGuiCol.BorderShadow]           = new (0.15f, 0.15f, 0.15f, 0.00f);
        colors[(int)ImGuiCol.FrameBg]                = new (0.42f, 0.42f, 0.42f, 0.50f);
        colors[(int)ImGuiCol.FrameBgHovered]         = new (0.45f, 0.63f, 0.98f, 0.62f);
        colors[(int)ImGuiCol.FrameBgActive]          = new (0.46f, 0.46f, 0.46f, 0.62f);
        colors[(int)ImGuiCol.TitleBg]                = new (0.04f, 0.04f, 0.04f, 1.00f);
        colors[(int)ImGuiCol.TitleBgActive]          = new (0.00f, 0.00f, 0.00f, 0.47f);
        colors[(int)ImGuiCol.TitleBgCollapsed]       = new (0.16f, 0.26f, 0.47f, 1.00f);
        colors[(int)ImGuiCol.MenuBarBg]              = new (0.27f, 0.27f, 0.28f, 0.74f);
        colors[(int)ImGuiCol.ScrollbarBg]            = new (0.27f, 0.27f, 0.28f, 0.55f);
        colors[(int)ImGuiCol.ScrollbarGrab]          = new (0.40f, 0.44f, 0.51f, 0.47f);
        colors[(int)ImGuiCol.ScrollbarGrabHovered]   = new (0.22f, 0.28f, 0.41f, 1.00f);
        colors[(int)ImGuiCol.ScrollbarGrabActive]    = new (0.14f, 0.18f, 0.26f, 0.84f);
        colors[(int)ImGuiCol.CheckMark]              = new (0.88f, 0.88f, 0.88f, 0.76f);
        colors[(int)ImGuiCol.SliderGrab]             = new (0.68f, 0.68f, 0.68f, 0.57f);
        colors[(int)ImGuiCol.SliderGrabActive]       = new (0.29f, 0.29f, 0.29f, 0.77f);
        colors[(int)ImGuiCol.Button]                 = new (0.33f, 0.34f, 0.35f, 0.45f);
        colors[(int)ImGuiCol.ButtonHovered]          = new (0.22f, 0.28f, 0.41f, 1.00f);
        colors[(int)ImGuiCol.ButtonActive]           = new (0.14f, 0.18f, 0.26f, 1.00f);
        colors[(int)ImGuiCol.Header]                 = new (0.46f, 0.47f, 0.50f, 0.49f);
        colors[(int)ImGuiCol.HeaderHovered]          = new (0.45f, 0.63f, 0.98f, 0.62f);
        colors[(int)ImGuiCol.HeaderActive]           = new (0.46f, 0.46f, 0.46f, 0.62f);
        colors[(int)ImGuiCol.Separator]              = new (0.31f, 0.31f, 0.31f, 1.00f);
        colors[(int)ImGuiCol.SeparatorHovered]       = new (0.31f, 0.31f, 0.31f, 1.00f);
        colors[(int)ImGuiCol.SeparatorActive]        = new (0.31f, 0.31f, 0.31f, 1.00f);
        colors[(int)ImGuiCol.ResizeGrip]             = new (0.98f, 0.98f, 0.98f, 0.78f);
        colors[(int)ImGuiCol.ResizeGripHovered]      = new (0.98f, 0.98f, 0.98f, 0.55f);
        colors[(int)ImGuiCol.ResizeGripActive]       = new (0.98f, 0.98f, 0.98f, 0.83f);
        colors[(int)ImGuiCol.Tab]                    = new (0.18f, 0.31f, 0.57f, 0.79f);
        colors[(int)ImGuiCol.TabHovered]             = new (0.26f, 0.50f, 0.96f, 0.74f);
        colors[(int)ImGuiCol.TabSelected]            = new (0.20f, 0.36f, 0.67f, 1.00f);
        colors[(int)ImGuiCol.TabDimmed]              = new (0.07f, 0.09f, 0.14f, 0.89f);
        colors[(int)ImGuiCol.TabDimmedSelected]      = new (0.13f, 0.23f, 0.42f, 1.00f);
        colors[(int)ImGuiCol.DockingPreview]         = new (0.26f, 0.50f, 0.96f, 0.64f);
        colors[(int)ImGuiCol.DockingEmptyBg]         = new (0.20f, 0.20f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.PlotLines]              = new (0.60f, 0.60f, 0.60f, 1.00f);
        colors[(int)ImGuiCol.PlotLinesHovered]       = new (0.35f, 0.56f, 0.98f, 1.00f);
        colors[(int)ImGuiCol.PlotHistogram]          = new (0.01f, 0.30f, 0.88f, 1.00f);
        colors[(int)ImGuiCol.PlotHistogramHovered]   = new (0.01f, 0.34f, 0.98f, 1.00f);
        colors[(int)ImGuiCol.TableHeaderBg]          = new (0.18f, 0.19f, 0.20f, 1.00f);
        colors[(int)ImGuiCol.TableBorderStrong]      = new (0.30f, 0.32f, 0.34f, 1.00f);
        colors[(int)ImGuiCol.TableBorderLight]       = new (0.22f, 0.23f, 0.24f, 1.00f);
        colors[(int)ImGuiCol.TableRowBg]             = new (0.00f, 0.00f, 0.00f, 0.00f);
        colors[(int)ImGuiCol.TableRowBgAlt]          = new (0.98f, 0.98f, 0.98f, 0.06f);
        colors[(int)ImGuiCol.TextSelectedBg]         = new (0.18f, 0.39f, 0.78f, 0.83f);
        colors[(int)ImGuiCol.DragDropTarget]         = new (0.01f, 0.34f, 0.98f, 0.83f);
        colors[(int)ImGuiCol.NavHighlight]           = new (0.26f, 0.50f, 0.96f, 1.00f);
        colors[(int)ImGuiCol.NavWindowingHighlight]  = new (0.98f, 0.98f, 0.98f, 0.64f);
        colors[(int)ImGuiCol.NavWindowingDimBg]      = new (0.78f, 0.78f, 0.78f, 0.20f);
        colors[(int)ImGuiCol.ModalWindowDimBg]       = new (0.78f, 0.78f, 0.78f, 0.35f);

        style.WindowPadding                          = new(8, 8);
        style.FramePadding                           = new(4, 3);
        style.CellPadding                            = new(4, 2);
        style.ItemSpacing                            = new(8, 4);
        style.ItemInnerSpacing                       = new(4, 4);
        style.TouchExtraPadding                      = new(0, 0);
        style.IndentSpacing                          = 21;
        style.ScrollbarSize                          = 14;
        style.GrabMinSize                            = 12;
                                                     
        style.WindowBorderSize                       = 1;
        style.ChildBorderSize                        = 1;
        style.PopupBorderSize                        = 1;
        style.FrameBorderSize                        = 0;
        style.TabBorderSize                          = 0;
                                                     
        style.WindowRounding                         = 3;
        style.ChildRounding                          = 3;
        style.FrameRounding                          = 3;
        style.PopupRounding                          = 3;
        style.ScrollbarRounding                      = 12;
        style.GrabRounding                           = 3;
        style.LogSliderDeadzone                      = 4;
        style.TabRounding                            = 3;
                                                     
        style.WindowTitleAlign                       = new (0, 0.5f);
        style.WindowMenuButtonPosition               = ImGuiDir.Left;
        style.ColorButtonPosition                    = ImGuiDir.Right;
        style.ButtonTextAlign                        = new (0.5f, 0.5f);
        style.SelectableTextAlign                    = new (0,0);
        style.DisplaySafeAreaPadding                 = new (3,3);

        return style;
    }
}