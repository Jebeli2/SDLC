// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IParticleEffect
{
    int TotalParticleCount { get; set; }
    int ParticleCount { get; }
    float X { get; set; }
    float Y { get; set; }   

    void Render(IRenderer renderer);
    void Update(float deltaTime);
}
