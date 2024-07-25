using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using ImGuiNET;

namespace Farkle.Services.UI;

public class GuiFontProvider
{
    private readonly Dictionary<GuiFontStyle, ImFontPtr> _fonts = new Dictionary<GuiFontStyle, ImFontPtr>();
    private ResourceManager _resourceManager;

    public GuiFontProvider(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
    }

    public ImFontPtr GetFontPtr(GuiFontStyle fontStyle)
    {
        if (!_fonts.TryGetValue(fontStyle, out var fontPtr)) 
            throw new ArgumentException("No font loaded for specified font style.", nameof(fontStyle));

        return fontPtr;
    }

    public static string GetFontResourceName(GuiFontStyle font)
    {
        return font switch
        {
            GuiFontStyle.Regular or GuiFontStyle.RegularLarge => "Roboto-Regular",
            GuiFontStyle.Bold or GuiFontStyle.BoldLarge => "Roboto-Bold",
            GuiFontStyle.MonoRegular or GuiFontStyle.MonoRegularLarge => "Inconsolata-Regular",
            GuiFontStyle.MonoBold or GuiFontStyle.MonoBoldLarge => "Inconsolata-Bold",
            _ => throw new InvalidEnumArgumentException(nameof(font), (int)font, typeof(GuiFontStyle))
        };
    }

    public string GetFontFilePath(GuiFontStyle font)
    {
        var fontName = GetFontResourceName(font);
        return GetFontFilePath(fontName);
    }

    private string GetFontFilePath(string fontName)
    {
        if (!fontName.EndsWith(".ttf", StringComparison.OrdinalIgnoreCase)) 
            fontName = $"{fontName}.ttf";
            
        var resourceName = $"Fonts/{fontName}";
        var assemblyPath = Assembly.GetExecutingAssembly().Location;
        var assemblyDirectory = Path.GetDirectoryName(assemblyPath)!;
        var contentDirectory = Path.Combine(assemblyDirectory, _resourceManager.ContentRootDirectory);
        var filePath = Path.Combine(contentDirectory, resourceName);

        ExceptionHelper.ThrowIfFileNotFound(filePath);

        return filePath;
    }

    public ImFontPtr AddFont(GuiFontStyle fontStyle, float sizePixels)
    {
        var filePath = GetFontFilePath(fontStyle);
        return ImGui.GetIO().Fonts.AddFontFromFileTTF(filePath, sizePixels);
    }

    public void LoadAllFonts(ImGuiIOPtr io)
    {
        _fonts.Add(GuiFontStyle.Regular,          AddFont(GuiFontStyle.Regular, NormalFontSize));
        _fonts.Add(GuiFontStyle.Bold,             AddFont(GuiFontStyle.Bold, NormalFontSize));
        _fonts.Add(GuiFontStyle.MonoRegular,      AddFont(GuiFontStyle.MonoRegular, NormalFontSize));
        _fonts.Add(GuiFontStyle.MonoBold,         AddFont(GuiFontStyle.MonoBold, NormalFontSize));

        _fonts.Add(GuiFontStyle.RegularLarge,     AddFont(GuiFontStyle.RegularLarge, LargeFontSize));
        _fonts.Add(GuiFontStyle.BoldLarge,        AddFont(GuiFontStyle.BoldLarge, LargeFontSize));
        _fonts.Add(GuiFontStyle.MonoRegularLarge, AddFont(GuiFontStyle.MonoRegularLarge, LargeFontSize));
        _fonts.Add(GuiFontStyle.MonoBoldLarge,    AddFont(GuiFontStyle.MonoBoldLarge, LargeFontSize));
    }

    public const int NormalFontSize = 24;
    public const int LargeFontSize = 30;
}

public enum GuiFontStyle
{
    Regular,
    RegularLarge,
    Bold,
    BoldLarge,
    MonoRegular,
    MonoRegularLarge,
    MonoBold,
    MonoBoldLarge
}