namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Resources;
    using System.Text;
    using System.Threading.Tasks;

    internal class SDLContentManager : IContentManager
    {
        private readonly List<ResourceManager> resourceManagers = new();
        private readonly SDLWindow window;
        private readonly List<string> knownNames = new();
        private bool allowFromFileSystem = true;

        internal SDLContentManager(SDLWindow window)
        {
            this.window = window;
        }

        public void AddResourceManager(ResourceManager resourceManager)
        {
            if (!resourceManagers.Contains(resourceManager))
            {
                resourceManagers.Add(resourceManager);
                knownNames.AddRange(ListResources(resourceManager));
            }
        }

        public byte[]? FindContent(string? name)
        {
            byte[]? data = null;
            if (!string.IsNullOrEmpty(name))
            {
                if (allowFromFileSystem)
                {
                    data = FindInFileSystem(name);
                    if (data != null) return data;
                }
                data = FindInResManagers(name);
                if (data != null) return data;
            }
            return null;
        }


        private byte[]? FindInResManagers(string name)
        {
            name = FindResName(name);
            foreach (ResourceManager rm in resourceManagers)
            {
                object? obj = rm.GetObject(name);
                if (obj != null)
                {
                    if (obj is byte[] data) { return data; }
                    else if (obj is string str)
                    {
                        return Encoding.UTF8.GetBytes(str);
                    }
                }
                else
                {
                    UnmanagedMemoryStream? ums = rm.GetStream(name);
                    if (ums != null)
                    {
                        byte[] umsData = new byte[ums.Length];
                        ums.Read(umsData, 0, umsData.Length);
                        return umsData;
                    }
                }
            }
            return null;
        }

        private static byte[]? FindInFileSystem(string name)
        {
            try
            {
                if (File.Exists(name))
                {
                    return File.ReadAllBytes(name);
                }
            }
            catch (IOException ioe)
            {
                SDLLog.Warn(LogCategory.SYSTEM, $"IOException during file read for '{name}': {ioe.Message}");
            }
            return null;
        }

        private string FindResName(string name)
        {
            if (knownNames.Contains(name)) return name;
            string testName = name.Replace('_', '-');
            if (knownNames.Contains(testName)) return testName;
            return name;
        }

        private static string AdjustFileNameForResManager(string fileName, bool replaceUnderscores = true)
        {
            if (string.IsNullOrEmpty(fileName)) return string.Empty;
            string fn = Path.GetFileName(fileName);
            fn = FileUtils.RemoveFileExtension(fn);
            if (replaceUnderscores) { fn = fn.Replace('_', '-'); }
            return fn;
        }
        private static IEnumerable<string> ListResources(ResourceManager rm)
        {
            ResourceSet? rs = rm.GetResourceSet(System.Globalization.CultureInfo.InvariantCulture, true, false);
            if (rs != null)
            {
                foreach (System.Collections.DictionaryEntry e in rs)
                {
                    string? s = e.Key?.ToString();
                    if (!string.IsNullOrEmpty(s))
                    {
                        yield return s;
                    }
                }
            }
        }

    }
}
