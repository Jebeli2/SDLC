// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDLC.Particles;

public interface IParticleSystem
{
    float FixedDelta { get; set; }
    bool UseFixedDelta { get; set; }
    void AddParticleEffect(Style style, float x, float y);
    void Clear();
}
