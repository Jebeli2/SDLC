// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ParticleEffect
{
    private readonly List<ParticleEmitter> emitters = new();

    public void Update(float elapsedTime)
    {
        foreach (ParticleEmitter emitter in emitters)
        {
            emitter.Update(elapsedTime);
        }
    }

    public void Render(IRenderer renderer)
    {
        foreach (ParticleEmitter emitter in emitters)
        {
            emitter.Render(renderer);
        }
    }
}
