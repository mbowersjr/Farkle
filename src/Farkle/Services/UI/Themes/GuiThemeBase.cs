using ImGuiNET;

namespace Farkle.Services.UI.Themes;

public abstract class GuiThemeBase
{
    public virtual float DefaultThemeHue { get; } = 0f;

    public abstract ImGuiStylePtr ApplyTheme();

    public virtual ImGuiStylePtr ApplyTheme(float hue)
    {
        var style = ApplyTheme();
        ApplyHue(style, hue);
        
        return style;
    }

    protected void ApplyHue(ImGuiStylePtr style, float hue)
    {
        for (var i = 0; i < (int)ImGuiCol.COUNT; i++)
        {
            var col = style.Colors[i];
            float h, s, v;

            ImGui.ColorConvertRGBtoHSV(col.X, col.Y, col.Z, out h, out s, out v);
            h = hue;
            ImGui.ColorConvertHSVtoRGB(h, s, v, out col.X, out col.Y, out col.Z);
        }
    }

}