namespace SDLC
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IContentManager
    {
        void AddResourceManager(System.Resources.ResourceManager resourceManager);
        byte[]? FindContent(string? name);
    }
}
