// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDLC.Graphics;

public class ParticleEffect
{
    private readonly List<ParticleEmitter> emitters = new();
    private readonly IRenderer renderer;
    public ParticleEffect(IRenderer renderer)
    {
        this.renderer = renderer;
    }

    public void Start()
    {
        foreach(ParticleEmitter emitter in emitters) { emitter.Start(); }
    }

    public void Reset(bool resetScaling = true)
    {
        foreach(ParticleEmitter emitter in emitters) { emitter.Reset(); }
    }

    public void SetPosition(float x, float y)
    {
        foreach(ParticleEmitter emitter in emitters) { emitter.SetPosition(x, y); } 
    }
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

    public void Load(string fileName)
    {
        var reader = File.OpenText(fileName);
        Load(reader);
        LoadEmitterImages(fileName);
    }

    public void Load(TextReader reader)
    {
        emitters.Clear();
        while (true)
        {
            ParticleEmitter emitter = NewEmitter(reader);
            emitters.Add(emitter);
            if (reader.ReadLine() == null) break;
        }
    }

    private void LoadEmitterImages(string fileName)
    {
        string? dir = Path.GetDirectoryName(fileName);
        if (dir != null)
        {
            foreach (ParticleEmitter emitter in emitters)
            {
                if (emitter.ImagePaths.Count == 0) { continue; }
                List<TextureRegion> sprites = new List<TextureRegion>();
                foreach (string imagePath in emitter.ImagePaths)
                {
                    string fn = Path.Combine(dir, Path.GetFileName(imagePath));
                    SDLTexture? tx = renderer.LoadTexture(fn);
                    if (tx != null)
                    {
                        sprites.Add(new TextureRegion(tx));
                    }
                }
                emitter.Sprites = sprites;
            }
        }
    }

    protected ParticleEmitter NewEmitter(TextReader reader)
    {
        return new ParticleEmitter(reader);
    }
}
