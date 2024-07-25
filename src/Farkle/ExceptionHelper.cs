using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Farkle;

public static class ExceptionHelper
{
    public static void ThrowIfFileNotFound([NotNull] string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"File not found: {path}", path);
    }

    public static void ThrowIfDirectoryNotFound([NotNull] string path)
    {
        if (!Directory.Exists(path))
            throw new DirectoryNotFoundException($"Directory not found: {path}");
    }
}