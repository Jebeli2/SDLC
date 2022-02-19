// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;

using System;

internal static class FileUtils
{
    public static string GetTempFile(string name, bool tryDelete, int maxTries = 9)
    {
        string path = Path.GetTempPath();
        string? ext = GetFileExtension(name);
        string simpleName = RemoveFileExtension(name).Replace('/', '_').Replace('\\', '_');
        string testName = Path.Combine(path, simpleName);
        string fileName = AddFileExtension(testName, ext);
        if (!File.Exists(fileName)) return fileName;
        for (int num = 1; num <= maxTries; num++)
        {
            if (tryDelete)
            {
                try
                {
                    File.Delete(fileName);
                    return fileName;
                }
                catch (Exception)
                {

                }
            }
            testName = Path.Combine(path, simpleName + "(" + num + ")");
            fileName = AddFileExtension(testName, ext);
            if (!File.Exists(fileName)) return fileName;
        }
        return Path.GetTempFileName();
    }

    public static string? GetFileExtension(string? name)
    {
        if (name == null) return null;
        int index = name.LastIndexOf('.');
        if (index >= 0) { return name[(index + 1)..]; }
        return string.Empty;
    }

    public static string RemoveFileExtension(string? name)
    {
        if (name == null) return string.Empty;
        int index = name.LastIndexOf('.');
        if (index >= 0) { return name[..index]; }
        return name;
    }

    public static string AddFileExtension(string? name, string? ext)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;
        if (string.IsNullOrEmpty(ext)) return name;
        return name + '.' + ext;
    }

    public static bool ReadBoolean(this TextReader reader, string name)
    {
        return bool.Parse(ReadString(reader,name));
    }
    public static int ReadInt(this TextReader reader, string name)
    {
        return int.Parse(ReadString(reader, name));
    }
    public static float ReadFloat(this TextReader reader, string name)
    {
        return float.Parse(ReadString(reader, name));
    }

    public static string ReadString(this TextReader reader, string name)
    {
        string? line = reader.ReadLine();
        if (line == null) throw new IOException($"Missing value: {name}");
        return ReadString(line);
    }

    public static string ReadString(string line)
    {
        return line.Substring(line.IndexOf(':') + 1).Trim();
    }
}
