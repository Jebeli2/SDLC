// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Particles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// C# Port of SDL2-particles by scarsty: "https://github.com/scarsty/SDL2-particles".
/// </summary>
public class ParticleSystem : SDLApplet, IParticleSystem
{
    private readonly List<IParticleEffect> effects = new();
    private SDLTexture? texture;
    private float fixedDelta = 1.0f / 25;
    private bool useFixedDelta = true;
    public ParticleSystem() : base("Particle System")
    {

    }

    public float FixedDelta { get => fixedDelta; set => fixedDelta = value; }
    public bool UseFixedDelta { get => useFixedDelta; set => useFixedDelta = value; }

    protected override void OnWindowLoad(SDLWindowLoadEventArgs e)
    {
        texture = LoadTexture("fire", Properties.Resources.fire);
    }

    protected override void OnDispose()
    {
        texture?.Dispose();
    }

    protected override void OnWindowPaint(SDLWindowPaintEventArgs e)
    {
        foreach (IParticleEffect effect in effects)
        {
            effect.Render(e.Renderer);
        }
    }

    protected override void OnWindowUpdate(SDLWindowUpdateEventArgs e)
    {
        float dT = useFixedDelta ? fixedDelta : (float)(e.ElapsedTime / 1000.0);
        foreach (IParticleEffect effect in effects)
        {
            effect.Update(dT);
        }
    }

    public void Clear()
    {
        effects.Clear();
    }

    public void AddParticleEffect(Style style, float x, float y)
    {
        SimpleParticleEffect spe;
        switch (style)
        {
            default:
            case Style.None:

                break;
            case Style.Fire:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 250;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = PointF.Empty;
                spe.RadialAccel = 0;
                spe.RadialAccelVar = 0;
                spe.Speed = -60;
                spe.SpeedVar = 20;
                spe.Angle = 90;
                spe.AngleVar = 10;
                spe.Life = 3;
                spe.LifeVar = 0.25f;
                spe.StartSize = 54.0f;
                spe.StartSizeVar = 10.0f;
                spe.EndSize = -1;
                spe.EmissionRate = spe.TotalParticleCount / spe.Life;
                spe.StartColor = new Graphics.ColorF(0.76f, 0.25f, 0.12f, 1.0f);
                spe.PosVar = new PointF(40.0f, 20.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.FireWork:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 1500;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = new PointF(0.0f, 90.0f);
                spe.RadialAccel = 0;
                spe.RadialAccelVar = 0;
                spe.Speed = -180;
                spe.SpeedVar = 50;
                spe.Angle = 90;
                spe.AngleVar = 20;
                spe.Life = 3.5f;
                spe.LifeVar = 1.0f;
                spe.StartSize = 8.0f;
                spe.StartSizeVar = 2.0f;
                spe.EndSize = -1;
                spe.EmissionRate = spe.TotalParticleCount / spe.Life;
                spe.StartColor = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 0.1f);
                spe.EndColor = new Graphics.ColorF(0.1f, 0.1f, 0.1f, 0.2f);
                spe.EndColorVar = new Graphics.ColorF(0.1f, 0.1f, 0.1f, 0.2f);
                spe.PosVar = new PointF(0.0f, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.Sun:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 350;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = new PointF(0.0f, 0.0f);
                spe.RadialAccel = 0;
                spe.RadialAccelVar = 0;
                spe.Speed = -20;
                spe.SpeedVar = 5;
                spe.Angle = 90;
                spe.AngleVar = 360;
                spe.Life = 1.0f;
                spe.LifeVar = 0.5f;
                spe.StartSize = 30.0f;
                spe.StartSizeVar = 10.0f;
                spe.EndSize = -1;
                spe.EmissionRate = spe.TotalParticleCount / spe.Life;
                spe.StartColor = new Graphics.ColorF(0.76f, 0.25f, 0.12f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.EndColor = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 1.0f);
                spe.EndColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.PosVar = new PointF(0.0f, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.Galaxy:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 200;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = new PointF(0.0f, 0.0f);
                spe.RadialAccel = -80;
                spe.RadialAccelVar = 0;
                spe.TangentialAccel = 80;
                spe.TangentialAccelVar = 0;
                spe.Speed = -60;
                spe.SpeedVar = 10;
                spe.Angle = 90;
                spe.AngleVar = 360;
                spe.Life = 4.0f;
                spe.LifeVar = 1.0f;
                spe.StartSize = 37.0f;
                spe.StartSizeVar = 10.0f;
                spe.EndSize = -1;
                spe.EmissionRate = spe.TotalParticleCount / spe.Life;
                spe.StartColor = new Graphics.ColorF(0.12f, 0.25f, 0.76f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.EndColor = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 1.0f);
                spe.EndColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.PosVar = new PointF(0.0f, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.Meteor:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 150;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = new PointF(-200, -200);
                spe.Speed = -15;
                spe.SpeedVar = 5;
                spe.RadialAccel = 0;
                spe.RadialAccelVar = 0;
                spe.TangentialAccel = 0;
                spe.TangentialAccelVar = 0;
                spe.Angle = 90;
                spe.AngleVar = 360;
                spe.Life = 2;
                spe.LifeVar = 1;
                spe.StartSize = 60.0f;
                spe.StartSizeVar = 10.0f;
                spe.EndSize = -1;
                spe.EmissionRate = spe.TotalParticleCount / spe.Life;
                spe.StartColor = new Graphics.ColorF(0.2f, 0.4f, 0.7f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.2f, 0.1f);
                spe.EndColor = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 1.0f);
                spe.EndColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.PosVar = new PointF(0.0f, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.Flower:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 250;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = PointF.Empty;
                spe.Speed = -80;
                spe.SpeedVar = 10;
                spe.RadialAccel = -60;
                spe.RadialAccelVar = 0;
                spe.TangentialAccel = 15;
                spe.TangentialAccelVar = 0;
                spe.Angle = 90;
                spe.AngleVar = 360;
                spe.Life = 4;
                spe.LifeVar = 1;
                spe.StartSize = 30.0f;
                spe.StartSizeVar = 10.0f;
                spe.EndSize = -1;
                spe.EmissionRate = spe.TotalParticleCount / spe.Life;
                spe.StartColor = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 0.5f);
                spe.EndColor = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 1.0f);
                spe.EndColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.PosVar = new PointF(0.0f, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.Spiral:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 500;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = PointF.Empty;
                spe.Speed = -150;
                spe.SpeedVar = 0;
                spe.RadialAccel = -380;
                spe.RadialAccelVar = 0;
                spe.TangentialAccel = 45;
                spe.TangentialAccelVar = 0;
                spe.Angle = 90;
                spe.AngleVar = 0;
                spe.Life = 12;
                spe.LifeVar = 0;
                spe.StartSize = 20.0f;
                spe.StartSizeVar = 0.0f;
                spe.EndSize = -1;
                spe.EmissionRate = spe.TotalParticleCount / spe.Life;
                spe.StartColor = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 0.0f);
                spe.EndColor = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 1.0f);
                spe.EndColorVar = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 0.0f);
                spe.PosVar = new PointF(0.0f, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.Explosion:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 700;
                spe.Duration = 0.1f;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = PointF.Empty;
                spe.Speed = -70;
                spe.SpeedVar = 40;
                spe.RadialAccel = 0;
                spe.RadialAccelVar = 0;
                spe.TangentialAccel = 0;
                spe.TangentialAccelVar = 0;
                spe.Angle = 90;
                spe.AngleVar = 360;
                spe.Life = 5.0f;
                spe.LifeVar = 2;
                spe.StartSize = 15.0f;
                spe.StartSizeVar = 10.0f;
                spe.EndSize = -1;
                spe.EmissionRate = spe.TotalParticleCount / spe.Life;
                spe.StartColor = new Graphics.ColorF(0.7f, 0.1f, 0.2f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 0.0f);
                spe.EndColor = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 0.0f);
                spe.EndColorVar = new Graphics.ColorF(0.5f, 0.5f, 0.5f, 0.0f);
                spe.PosVar = new PointF(0.0f, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.Smoke:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 200;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = PointF.Empty;
                spe.Speed = -25;
                spe.SpeedVar = 10;
                spe.RadialAccel = 0;
                spe.RadialAccelVar = 0;
                spe.TangentialAccel = 0;
                spe.TangentialAccelVar = 0;
                spe.Angle = 90;
                spe.AngleVar = 5;
                spe.Life = 4.0f;
                spe.LifeVar = 1;
                spe.StartSize = 60.0f;
                spe.StartSizeVar = 10.0f;
                spe.EndSize = -1;
                spe.EmissionRate = spe.TotalParticleCount / spe.Life;
                spe.StartColor = new Graphics.ColorF(0.8f, 0.8f, 0.8f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.02f, 0.02f, 0.02f, 0.0f);
                spe.EndColor = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 1.0f);
                spe.EndColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.PosVar = new PointF(20.0f, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.Snow:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 700;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = new PointF(0, 1);
                spe.Speed = -5;
                spe.SpeedVar = 1;
                spe.RadialAccel = 0;
                spe.RadialAccelVar = 1;
                spe.TangentialAccel = 0;
                spe.TangentialAccelVar = 1;
                spe.Angle = -90;
                spe.AngleVar = 5;
                spe.Life = 45.0f;
                spe.LifeVar = 15;
                spe.StartSize = 10.0f;
                spe.StartSizeVar = 5.0f;
                spe.EndSize = -1;
                spe.EmissionRate = 10;
                spe.StartColor = new Graphics.ColorF(1.0f, 1.0f, 1.0f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.EndColor = new Graphics.ColorF(1.0f, 1.0f, 1.0f, 0.0f);
                spe.EndColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.PosVar = new PointF(1.0f * x, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
            case Style.Rain:
                spe = new SimpleParticleEffect();
                spe.Texture = texture;
                spe.X = x;
                spe.Y = y;
                spe.TotalParticleCount = 1000;
                spe.Duration = -1;
                spe.EmitterMode = Mode.Gravity;
                spe.Gravity = new PointF(10, 10);
                spe.Speed = -130;
                spe.SpeedVar = 30;
                spe.RadialAccel = 0;
                spe.RadialAccelVar = 1;
                spe.TangentialAccel = 0;
                spe.TangentialAccelVar = 1;
                spe.Angle = -90;
                spe.AngleVar = 5;
                spe.Life = 4.5f;
                spe.LifeVar = 0;
                spe.StartSize = 4.0f;
                spe.StartSizeVar = 2.0f;
                spe.EndSize = -1;
                spe.EmissionRate = 20;
                spe.StartColor = new Graphics.ColorF(0.7f, 0.8f, 1.0f, 1.0f);
                spe.StartColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.EndColor = new Graphics.ColorF(0.7f, 0.8f, 1.0f, 0.5f);
                spe.EndColorVar = new Graphics.ColorF(0.0f, 0.0f, 0.0f, 0.0f);
                spe.PosVar = new PointF(1.0f * x, 0.0f);
                spe.StartSpin = 0;
                spe.StartSpinVar = 90;
                spe.EndSpin = 90;
                spe.EndSpinVar = 90;
                spe.Active = true;
                effects.Add(spe);
                break;
        }
    }
}
