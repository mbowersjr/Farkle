using System;
using System.IO;
using FontStashSharp;

namespace Farkle.Services;

public class FontResource : IDisposable
{
    public string Name { get; private set; }
    public string FilePath { get; private set; }
    
    private FontSystem _fontSystem;
    
    public FontResource(string name, string filePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        Name = name;
        FilePath = filePath;
        
        LoadFont();
    }

    private void LoadFont()
    {
        ExceptionHelper.ThrowIfFileNotFound(FilePath);

        _fontSystem = new FontSystem();
        _fontSystem.AddFont(File.ReadAllBytes(FilePath));
    }

    public SpriteFontBase GetFont(float size)
    {
        return _fontSystem.GetFont(size);
    }

    public void Dispose()
    {
        _fontSystem?.Dispose();
    }
}