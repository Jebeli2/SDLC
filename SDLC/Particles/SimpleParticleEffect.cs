// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Particles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using SDLC.Graphics;

/// <summary>
/// C# Port of SDL2-particles by scarsty: "https://github.com/scarsty/SDL2-particles".
/// </summary>
internal class SimpleParticleEffect : IParticleEffect
{
    private SDLTexture? texture;
    private static Random random = new();
    private float x;
    private float y;
    private ParticleData[] data = Array.Empty<ParticleData>();
    private int particleCount;
    private float life;
    private float lifeVar;
    private float angle;
    private float angleVar;
    private float duration;
    private PointF sourcePos;
    private PointF posVar;
    private float startSize;
    private float startSizeVar;
    private float endSize;
    private float endSizeVar;
    private ColorF startColor;
    private ColorF startColorVar;
    private ColorF endColor;
    private ColorF endColorVar;
    private float startSpin;
    private float startSpinVar;
    private float endSpin;
    private float endSpinVar;
    private float emissionRate;


    private PointF gravity;
    private float speed;
    private float speedVar;
    private float tangentialAccel;
    private float tangentialAccelVar;
    private float radialAccel;
    private float radialAccelVar;
    private bool rotationIsDir;

    private float startRadius;
    private float startRadiusVar;
    private float endRadius;
    private float endRadiusVar;
    private float rotatePerSecond;
    private float rotatePerSecondVar;

    private bool additive = true;
    private float elapsed;
    private bool active;
    private Mode emitterMode = Mode.Gravity;
    private bool paused;
    private int totalParticles;
    private int yCoordFlipped = 1;


    private float emitCounter;

    public int TotalParticleCount
    {
        get => totalParticles;
        set
        {
            if (data.Length != value)
            {
                totalParticles = value;
                ParticleData[] newData = new ParticleData[value];
                if (data.Length > 0 && newData.Length > 0)
                {
                    Array.Copy(data, newData, Math.Min(data.Length, newData.Length));
                }
                data = new ParticleData[value];
            }
        }
    }

    public SDLTexture? Texture { get => texture; set => texture = value; }
    public bool Active { get => active; set => active = value; }
    public int ParticleCount => particleCount;
    public float X { get => x; set => x = value; }
    public float Y { get => y; set => y = value; }

    public float Duration { get => duration; set => duration = value; }
    public Mode EmitterMode { get => emitterMode; set => emitterMode = value; }
    public PointF Gravity { get => gravity; set => gravity = value; }
    public float RadialAccel { get => radialAccel; set => radialAccel = value; }
    public float RadialAccelVar { get => radialAccelVar; set => radialAccelVar = value; }
    public float TangentialAccel { get => tangentialAccel; set => tangentialAccel = value; }
    public float TangentialAccelVar { get => tangentialAccelVar; set => tangentialAccelVar = value; }
    public float Speed { get => speed; set => speed = value; }
    public float SpeedVar { get => speedVar; set => speedVar = value; }
    public float Angle { get => angle; set => angle = value; }
    public float AngleVar { get => angleVar; set => angleVar = value; }
    public float Life { get => life; set => life = value; }
    public float LifeVar { get => lifeVar; set => lifeVar = value; }
    public float StartSize { get => startSize; set => startSize = value; }
    public float StartSizeVar { get => startSizeVar; set => startSizeVar = value; }
    public float EndSize { get => endSize; set => endSize = value; }
    public float EndSizeVar { get => endSizeVar; set => endSizeVar = value; }
    public ColorF StartColor { get => startColor; set => startColor = value; }
    public ColorF StartColorVar { get => startColorVar; set => startColorVar = value; }
    public ColorF EndColor { get => endColor; set => endColor = value; }
    public ColorF EndColorVar { get => endColorVar; set => endColorVar = value; }
    public float EmissionRate { get => emissionRate; set => emissionRate = value; }
    public PointF PosVar { get => posVar; set => posVar = value; }
    public float StartSpin { get => startSpin; set => startSpin = value; }
    public float StartSpinVar { get => startSpinVar; set => startSpinVar = value; }
    public float EndSpin { get => endSpin; set => endSpin = value; }
    public float EndSpinVar { get => endSpinVar; set => endSpinVar = value; }

    public void Render(IRenderer renderer)
    {
        if (texture == null) return;
        RenderSimple(renderer, texture);
        //RenderGeo(renderer, texture);
    }

    // TODO: Rotation?
    // BUT: It seems, SDL queues the RenderCopyEx calls as RenderGeometry commands anyway (at least for direct3d11),
    // so there's nothing to be gained by doing this 'by hand'.
    private void RenderGeo(IRenderer renderer, SDLTexture tex)
    {
        List<RectangleF> rects = new();
        List<Color> colors = new();

        for (int i = 0; i < particleCount; i++)
        {
            var p = data[i];
            if (p.size <= 0 || p.colorA <= 0)
            {
                continue;
            }
            Color c = GetColor(p.colorA, p.colorR, p.colorG, p.colorB);
            RectangleF r = new RectangleF(p.posX + p.startPosX - p.size / 2, p.posY + p.startPosY - p.size / 2, p.size, p.size);
            rects.Add(r);
            colors.Add(c);
        }
        renderer.DrawTextureRects(tex, rects, colors);
    }
    private void RenderSimple(IRenderer renderer, SDLTexture tex)
    {
        renderer.BlendMode = BlendMode.Blend;
        tex.BlendMode = BlendMode.Blend;
        for (int i = 0; i < particleCount; i++)
        {
            var p = data[i];
            if (p.size <= 0 || p.colorA <= 0)
            {
                continue;
            }
            Color c = GetColor(p.colorA, p.colorR, p.colorG, p.colorB);
            tex.ColorMod = c;
            tex.AlphaMod = c.A;
            RectangleF r = new RectangleF(p.posX + p.startPosX - p.size / 2, p.posY + p.startPosY - p.size / 2, p.size, p.size);
            renderer.DrawTexture(texture, r, p.rotation);
        }
    }

    public void Update(float deltaTime)
    {
        float dt = deltaTime;
        //float dt = 1.0f / 25;
        if (active && emissionRate > 0)
        {
            float rate = 1.0f / emissionRate;
            if (particleCount < totalParticles)
            {
                emitCounter += dt;
                if (emitCounter < 0.0f)
                {

                    emitCounter = 0.0f;
                }
            }
            int emitCount = (int)Math.Min(totalParticles - particleCount, emitCounter / rate);
            AddParticles(emitCount);
            emitCounter -= rate * emitCount;
            elapsed += dt;
            if (duration != -1 && duration < elapsed)
            {
                Stop();
            }
        }
        for (int i = 0; i < particleCount; ++i)
        {
            data[i].timeToLive -= dt;
        }
        for (int i = 0; i < particleCount; ++i)
        {
            if (data[i].timeToLive <= 0.0f)
            {
                data[i] = data[particleCount - 1];
                --particleCount;
            }
        }
        if (emitterMode == Mode.Gravity)
        {
            for (int i = 0; i < particleCount; ++i)
            {
                PointF tmp = PointF.Empty;
                PointF radial = PointF.Empty;
                if (data[i].posX != 0 || data[i].posY != 0)
                {
                    NormalizePoint(data[i].posX, data[i].posY, ref radial);
                }
                PointF tangential = new PointF(radial.Y, radial.X);
                radial.X *= data[i].radialAccel;
                radial.Y *= data[i].radialAccel;
                tangential.X *= -data[i].tangentialAccel;
                tangential.Y *= data[i].tangentialAccel;

                tmp.X = radial.X + tangential.X + gravity.X;
                tmp.Y = radial.Y + tangential.Y + gravity.Y;
                tmp.X *= dt;
                tmp.Y *= dt;
                data[i].dirX += tmp.X;
                data[i].dirY += tmp.Y;
                tmp.X = data[i].dirX * dt * yCoordFlipped;
                tmp.Y = data[i].dirY * dt * yCoordFlipped;
                data[i].posX += tmp.X;
                data[i].posY += tmp.Y;
            }
        }
        else if (emitterMode == Mode.Radius)
        {
            for (int i = 0; i < particleCount; ++i)
            {
                data[i].angle += data[i].degreesPerSecond * dt;
                data[i].radius += data[i].deltaRadius * dt;
                data[i].posX = -MathF.Cos(data[i].angle) * data[i].radius;
                data[i].posY = -MathF.Sin(data[i].angle) * data[i].radius * yCoordFlipped;
            }
        }
        for (int i = 0; i < particleCount; ++i)
        {
            data[i].colorR += data[i].deltaColorR * dt;
            data[i].colorG += data[i].deltaColorG * dt;
            data[i].colorB += data[i].deltaColorB * dt;
            data[i].colorA += data[i].deltaColorA * dt;
            data[i].size += data[i].deltaSize * dt;
            data[i].size = MathF.Max(0.0f, data[i].size);
            data[i].rotation += data[i].deltaRotation * dt;
        }
    }

    private void Stop()
    {
        active = false;
        elapsed = duration;
        emitCounter = 0;
    }

    private void AddParticles(int count)
    {
        if (paused) return;
        seed = (uint)random.Next();
        int start = particleCount;
        particleCount += count;
        for (int i = start; i < particleCount; ++i)
        {
            float theLife = life + lifeVar * RandomM11();
            data[i].timeToLive = MathF.Max(0.0f, theLife);
        }
        for (int i = start; i < particleCount; ++i)
        {
            data[i].posX = sourcePos.X + posVar.X * RandomM11();
        }
        for (int i = start; i < particleCount; ++i)
        {
            data[i].posY = sourcePos.Y + posVar.Y * RandomM11();
        }
        for (int i = start; i < particleCount; ++i)
        {
            data[i].colorR = MathUtils.Clamp(startColor.R + startColorVar.R * RandomM11(), 0.0f, 1.0f);
            data[i].colorG = MathUtils.Clamp(startColor.G + startColorVar.G * RandomM11(), 0.0f, 1.0f);
            data[i].colorB = MathUtils.Clamp(startColor.B + startColorVar.B * RandomM11(), 0.0f, 1.0f);
            data[i].colorA = MathUtils.Clamp(startColor.A + startColorVar.A * RandomM11(), 0.0f, 1.0f);

            data[i].deltaColorR = MathUtils.Clamp(endColor.R + endColorVar.R * RandomM11(), 0.0f, 1.0f);
            data[i].deltaColorR = (data[i].deltaColorR - data[i].colorR) / data[i].timeToLive;
            data[i].deltaColorG = MathUtils.Clamp(endColor.G + endColorVar.G * RandomM11(), 0.0f, 1.0f);
            data[i].deltaColorG = (data[i].deltaColorG - data[i].colorG) / data[i].timeToLive;
            data[i].deltaColorB = MathUtils.Clamp(endColor.B + endColorVar.B * RandomM11(), 0.0f, 1.0f);
            data[i].deltaColorB = (data[i].deltaColorB - data[i].colorB) / data[i].timeToLive;
            data[i].deltaColorA = MathUtils.Clamp(endColor.A + endColorVar.A * RandomM11(), 0.0f, 1.0f);
            data[i].deltaColorA = (data[i].deltaColorA - data[i].colorA) / data[i].timeToLive;
        }
        for (int i = start; i < particleCount; ++i)
        {
            data[i].size = startSize + startSizeVar * RandomM11();
            data[i].size = MathF.Max(0.0f, data[i].size);
        }
        if (endSize != -1)
        {
            for (int i = start; i < particleCount; ++i)
            {
                float theEndSize = endSize + endSizeVar * RandomM11();
                theEndSize = MathF.Max(0.0f, theEndSize);
                data[i].deltaSize = (theEndSize - data[i].size) / data[i].timeToLive;
            }
        }
        else
        {
            for (int i = start; i < particleCount; ++i)
            {
                data[i].deltaSize = 0.0f;
            }
        }
        for (int i = start; i < particleCount; ++i)
        {
            data[i].rotation = startSpin + startSpinVar * RandomM11();
        }
        for (int i = start; i < particleCount; ++i)
        {
            float endA = endSpin + endSpinVar * RandomM11();
            data[i].deltaRotation = (endA - data[i].rotation) / data[i].timeToLive;
        }
        for (int i = start; i < particleCount; ++i)
        {
            data[i].startPosX = x;
            data[i].startPosY = y;
        }
        if (emitterMode == Mode.Gravity)
        {
            for (int i = start; i < particleCount; ++i)
            {
                data[i].radialAccel = radialAccel + radialAccelVar * RandomM11();
            }
            for (int i = start; i < particleCount; ++i)
            {
                data[i].tangentialAccel = tangentialAccel + tangentialAccelVar * RandomM11();
            }
            if (rotationIsDir)
            {
                for (int i = start; i < particleCount; ++i)
                {
                    float a = Deg2Rad(angle + angleVar * RandomM11());
                    float s = speed + speedVar * RandomM11();
                    Vector2 v = new Vector2(MathF.Cos(a), MathF.Sin(a));
                    Vector2 dir = v * s;
                    data[i].dirX = dir.X;
                    data[i].dirY = dir.Y;
                    float ang = MathF.Atan2(dir.X, dir.Y);
                    data[i].rotation = -Rad2Deg(ang);
                }

            }
            else
            {
                for (int i = start; i < particleCount; ++i)
                {
                    float a = Deg2Rad(angle + angleVar * RandomM11());
                    float s = speed + speedVar * RandomM11();
                    Vector2 v = new Vector2(MathF.Cos(a), MathF.Sin(a));
                    Vector2 dir = v * s;
                    data[i].dirX = dir.X;
                    data[i].dirY = dir.Y;
                }
            }

        }
        else if (emitterMode == Mode.Radius)
        {
            for (int i = start; i < particleCount; ++i)
            {
                data[i].radius = startRadius + startRadiusVar * RandomM11();
            }
            for (int i = start; i < particleCount; ++i)
            {
                data[i].angle = Deg2Rad(angle + angleVar * RandomM11());
            }
            for (int i = start; i < particleCount; ++i)
            {
                data[i].degreesPerSecond = Deg2Rad(rotatePerSecond + rotatePerSecondVar * RandomM11());
            }
            if (endRadius == -1)
            {
                for (int i = start; i < particleCount; ++i)
                {
                    data[i].deltaRadius = 0.0f;
                }
            }
            else
            {
                for (int i = start; i < particleCount; ++i)
                {
                    float theEndRadius = endRadius + endRadiusVar * RandomM11();
                    data[i].deltaRadius = (theEndRadius - data[i].radius) / data[i].timeToLive;
                }
            }
        }
    }

    private static Color GetColor(float a, float r, float g, float b)
    {
        byte ba = (byte)((int)(a * 255) & 0xFF);
        byte br = (byte)((int)(r * 255) & 0xFF);
        byte bg = (byte)((int)(g * 255) & 0xFF);
        byte bb = (byte)((int)(b * 255) & 0xFF);
        return Color.FromArgb(ba, br, bg, bb);
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct uni
    {
        [FieldOffset(0)]
        public uint d;
        [FieldOffset(0)]
        public float f;
    }
    private static uni u = new();
    private static float RandomM11(ref uint seed)
    {
        seed = seed * 134775813 + 1;
        u.d = ((seed & 0x7FFF) << 8) | 0x40000000;
        float res = u.f - 3.0f;
        return res;
    }

    private static uint seed = (uint)random.Next();
    private static float RandomM11()
    {
        return RandomM11(ref seed);
    }
    private static float Deg2Rad(float a)
    {
        return a * 0.01745329252f;
    }

    private static float Rad2Deg(float a)
    {
        return a * 57.29577951f;
    }

    private static void NormalizePoint(float x, float y, ref PointF p)
    {
        float n = x * x + y * y;
        if (n == 1.0f) { return; }
        n = MathF.Sqrt(n);
        if (n < 1e-5f) { return; }
        n = 1.0f / n;
        p.X = x * n;
        p.Y = y * n;
    }

    private struct ParticleData
    {
        public float posX;
        public float posY;
        public float startPosX;
        public float startPosY;
        public float colorR;
        public float colorG;
        public float colorB;
        public float colorA;
        public float deltaColorR;
        public float deltaColorG;
        public float deltaColorB;
        public float deltaColorA;
        public float size;
        public float deltaSize;
        public float rotation;
        public float deltaRotation;
        public float timeToLive;

        public float dirX;
        public float dirY;
        public float radialAccel;
        public float tangentialAccel;

        public float angle;
        public float degreesPerSecond;
        public float radius;
        public float deltaRadius;

    }
}
