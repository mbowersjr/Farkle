using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FontStashSharp;
using MonoGame.Extended;

namespace Farkle.Services;

public class ResourceManager
{
    public string ContentRootDirectory => _game.Content.RootDirectory;
    
    private Dictionary<string, TextureResource> _textures = new Dictionary<string, TextureResource>();
    private Dictionary<string, FontResource> _fonts = new Dictionary<string, FontResource>();

    private GameMain _game;
    private bool _isLoaded;

    public ResourceManager(GameMain game)
    {
        _game = game;
    }
	    
    public void Load()
    {
        if (_isLoaded)
            return;

        var pixelTexture = new Texture2D(_game.GraphicsDevice, 1, 1);
        pixelTexture.SetData(new [] { Color.White });
        AddTexture("Pixel", pixelTexture);

        string fontsDirectory = Path.Combine(ContentRootDirectory, "Fonts");
        AddFontsInDirectory(fontsDirectory);
        
        _isLoaded = true;
    }

    public void AddFont(string path)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        
        string name = Path.GetFileNameWithoutExtension(path);
        AddFont(name, path);
    }

    public void AddFont(string name, string path, bool replaceIfExists = false)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        ArgumentException.ThrowIfNullOrEmpty(path);
        
        ExceptionHelper.ThrowIfFileNotFound(path);

        if (_fonts.ContainsKey(name) && !replaceIfExists) 
            throw new ArgumentException("A font with the same name already exists.", nameof(name));

        _fonts[name] = new FontResource(name, path);
    }

    public void AddFontsInDirectory(string path, string searchPattern = "*.ttf", SearchOption searchOption = SearchOption.TopDirectoryOnly)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);
        ExceptionHelper.ThrowIfDirectoryNotFound(path);

        string[] fontPaths = Directory.GetFiles(path, searchPattern, searchOption);

        foreach (string fontPath in fontPaths)
        {
            AddFont(fontPath);
        }
    }
    
    public void AddTexture(string name, Texture2D texture, bool replaceIfExists = false)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNull(texture);

        TextureResource textureResource = new TextureResource(name, texture);

        if (!_textures.TryAdd(name, textureResource))
        {
            if (!replaceIfExists)
                throw new ArgumentException("A texture with the same name already exists.", nameof(name));

            _textures[name] = textureResource;
        }
    }

    public FontResource GetFontResource([NotNull] string name)
    {
        if (!_fonts.TryGetValue(name, out FontResource fontResource))
        {
            throw new ArgumentException("A font with that name does not exist.", nameof(name));
        }

        return fontResource;
    }

    public SpriteFontBase GetFont([NotNull] string name, float size)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        FontResource fontResource = GetFontResource(name);
        return fontResource.GetFont(size);
    }

    public TextureResource GetTextureResource([NotNull] string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (!_textures.TryGetValue(name, out var textureResource))
        {
            throw new KeyNotFoundException($"Texture does not exist with name: {name}");
        }

        return textureResource;
    }

    public Texture2D GetTexture(string name)
    {
        TextureResource textureResource = GetTextureResource(name);
        return textureResource.Texture;
    }

    public void Unload()
    {
        if (!_isLoaded)
            return;

        foreach (TextureResource textureResource in _textures.Values)
        {
            textureResource.Dispose();
        }

        foreach (FontResource fontResource in _fonts.Values)
        {
            fontResource.Dispose();
        }

        _isLoaded = false;
    }


    private bool _disposed = false;
    public void Dispose()
    {
        if (_disposed)
            return;

        Unload();

        _disposed = true;
    }
}