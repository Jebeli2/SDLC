﻿// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
public interface IContentManager
{
    string SaveDirectory { get; }
    void AddResourceManager(System.Resources.ResourceManager resourceManager);
    byte[]? FindContent(string? name);
}
