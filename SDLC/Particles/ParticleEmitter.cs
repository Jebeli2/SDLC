// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDLC.Graphics;

public class ParticleEmitter
{
    [Flags]
    private enum UpdateFlags
    {
        Scale = 1,
        Angle = 2,
        Rotation = 4,
        Velocity = 8,
        Wind = 16,
        Gravity = 32,
        Tint = 64,
        Sprite = 128

    }

    private static readonly Random rng = new();

    private RangedNumericValue delayValue = new();
    private IndependentScaledNumericValue lifeOffsetValue = new();
    private RangedNumericValue durationValue = new();
    private IndependentScaledNumericValue lifeValue = new();
    private ScaledNumericValue emissionValue = new();
    private ScaledNumericValue xScaleValue = new();
    private ScaledNumericValue yScaleValue = new();
    private ScaledNumericValue rotationValue = new();
    private ScaledNumericValue velocityValue = new();
    private ScaledNumericValue angleValue = new();
    private ScaledNumericValue windValue = new();
    private ScaledNumericValue gravityValue = new();
    private ScaledNumericValue transparenyValue = new();
    private RangedNumericValue xOffsetValue = new();
    private RangedNumericValue yOffsetValue = new();
    private ScaledNumericValue spawnWidthValue = new();
    private ScaledNumericValue spawnHeightValue = new();

    private float accumulator;
    private int minParticleCount = 4;
    private int maxParticleCount = 4;
    private float x;
    private float y;
    private List<TextureRegion> sprites = new();
    private SpriteMode spriteMode = SpriteMode.Single;
    private Particle[] particles = new Particle[4];
    private bool[] active = new bool[4];
    private int activeCount;
    private bool firstUpdate;
    private float duration = 1.0f;
    private float durationTimer;
    private float delay;
    private float delayTimer;

    private bool attached;
    private bool continuous;
    private bool behind;
    private bool allowCompletion;
    private UpdateFlags updateFlags;
    private int emission;
    private int emissionDiff;
    private int emissionDelta;
    private int lifeOffset;
    private int lifeOffsetDiff;
    private int life;
    private int lifeDiff;
    private float spawnWidth;
    private float spawnWidthDiff;
    private float spawnHeight;
    private float spawnHeightDiff;

    private bool premultipliedAlpha = false;
    private bool additive = true;
    private bool cleansUpBlendFunction = true;

    public ParticleEmitter()
    {
        durationValue.AlwaysActive = true;
        emissionValue.AlwaysActive = true;
        lifeValue.AlwaysActive = true;
        xScaleValue.AlwaysActive = true;
        transparenyValue.AlwaysActive = true;
        spawnWidthValue.AlwaysActive = true;
        spawnHeightValue.AlwaysActive = true;
    }

    public int MaxParticleCount
    {
        get => maxParticleCount;
        set
        {
            maxParticleCount = value;
            active = new bool[maxParticleCount];
            activeCount = 0;
            particles = new Particle[maxParticleCount];
        }
    }


    public void Update(float elapsedTime)
    {
        accumulator += elapsedTime;
        if (accumulator < 1) return;
        int deltaMillis = (int)accumulator;
        accumulator -= deltaMillis;
        if (delayTimer < delay)
        {
            delayTimer += deltaMillis;
        }
        else
        {
            bool done = false;
            if (firstUpdate)
            {
                firstUpdate = false;
                AddParticle();
            }
            if (durationTimer < duration)
            {
                durationTimer += deltaMillis;
            }
            else
            {
                if (!continuous || allowCompletion)
                {
                    done = true;
                }
                else
                {
                    Restart();
                }
            }
            if (!done)
            {
                emissionDelta += deltaMillis;
                float emissionTime = emission + emissionDiff * emissionValue.GetScale(durationTimer / (float)duration);
                if (emissionTime > 0)
                {
                    emissionTime = 1000 / emissionTime;
                    if (emissionDelta >= emissionTime)
                    {
                        int emitCount = (int)(emissionDelta / emissionTime);
                        emitCount = Math.Min(emitCount, maxParticleCount - this.activeCount);
                        emissionDelta = (int)(emissionDelta - emitCount * emissionTime);
                        emissionDelta %= (int)emissionTime;
                        AddParticles(emitCount);

                    }
                }
                if (this.activeCount < minParticleCount) { AddParticles(minParticleCount - this.activeCount); }
            }
        }
        bool[] active = this.active;
        int activeCount = this.activeCount;
        Particle[] particles = this.particles;
        for (int i = 0, n = active.Length; i < n; i++)
        {
            if (active[i] && !UpdateParticle(particles[i], elapsedTime, deltaMillis))
            {
                active[i] = false;
                activeCount--;
            }
        }
        this.activeCount = activeCount;
    }

    public void Render(IRenderer renderer)
    {
        if (premultipliedAlpha)
        {
            renderer.BlendMode = BlendMode.Mul;
        }
        else if (additive)
        {
            renderer.BlendMode = BlendMode.Add;
        }
        else
        {
            renderer.BlendMode = BlendMode.Blend;
        }
        bool[] active = this.active;
        Particle[] particles = this.particles;
        for (int i = 0, n = active.Length; i < n; i++)
        {
            if (active[i]) { particles[i].Draw(renderer); }
        }
        if (cleansUpBlendFunction && (additive || premultipliedAlpha))
        {
            renderer.BlendMode = BlendMode.Blend;
        }
    }

    public void AddParticle()
    {
        int activeCount = this.activeCount;
        if (activeCount == maxParticleCount) return;
        bool[] active = this.active;
        for (int i = 0, n = active.Length; i < n; i++)
        {
            if (!active[i])
            {
                ActivateParticle(i);
                active[i] = true;
                this.activeCount = activeCount + 1;
                break;
            }
        }
    }

    public void AddParticles(int count)
    {
        count = Math.Min(count, maxParticleCount - activeCount);
        if (count == 0) return;
        bool[] active = this.active;
        int n = active.Length;
        int c = 0;
        for (int i = 0; i < n; i++)
        {
            if (!active[i])
            {
                ActivateParticle(i);
                active[i] = true;
                c++;
                if (c >= count) { break; }
            }

        }
        this.activeCount += count;
    }

    public void Start()
    {
        firstUpdate = true;
        allowCompletion = false;
        Restart();
    }

    public void Reset()
    {
        emissionDelta = 0;
        durationTimer = duration;
        activeCount = 0;
        Start();
    }

    private void Restart()
    {
        delay = delayValue.Active ? delayValue.NewLowValue() : 0;
        delayTimer = 0;
        durationTimer -= duration;
        duration = durationValue.NewLowValue();
        emission = (int)emissionValue.NewLowValue();
        emissionDiff = (int)emissionValue.NewHighValue();
        if (!emissionValue.Relative) { emissionDiff -= emission; }
        if (!lifeValue.Independent) { GenerateLifeValues(); }
        if (!lifeOffsetValue.Independent) { GenerateLifeOffsetValues(); }
        spawnWidth = spawnWidthValue.NewLowValue();
        spawnWidthDiff = spawnWidthValue.NewHighValue();
        if (!spawnWidthValue.Relative) { spawnWidthDiff -= spawnWidth; }

        spawnHeight = spawnHeightValue.NewLowValue();
        spawnHeightDiff = spawnHeightValue.NewHighValue();
        if (!spawnHeightValue.Relative) { spawnHeightDiff -= spawnHeight; }
        updateFlags = 0;
        if (angleValue.Active && angleValue.timeline.Length > 1) { updateFlags |= UpdateFlags.Angle; }
        if (velocityValue.Active) { updateFlags |= UpdateFlags.Velocity; }
        if (xScaleValue.timeline.Length > 1) { updateFlags |= UpdateFlags.Scale; }
        if (yScaleValue.Active && yScaleValue.timeline.Length > 1) { updateFlags |= UpdateFlags.Scale; }
        if (rotationValue.Active && rotationValue.timeline.Length > 1) { updateFlags |= UpdateFlags.Rotation; }
        if (windValue.Active) { updateFlags |= UpdateFlags.Wind; }
        if (gravityValue.Active) { updateFlags |= UpdateFlags.Gravity; }
        //if (tin)
        //if
    }
    protected virtual Particle NewParticle(TextureRegion sprite)
    {
        return new Particle(sprite);
    }

    private bool UpdateParticle(Particle particle, float delta, int deltaMillis)
    {
        return true;
    }

    private void ActivateParticle(int index)
    {
        TextureRegion sprite;
        switch (spriteMode)
        {
            case SpriteMode.Random:
                sprite = sprites[rng.Next(sprites.Count - 1)];
                break;
            case SpriteMode.Single:
            case SpriteMode.Animation:
            default:
                sprite = sprites[0];
                break;
        }
        Particle particle = particles[index];
        if (particle == null)
        {
            particle = NewParticle(sprite);
            particles[index] = particle;
        }
        else
        {
            particle.Set(sprite);
        }
        float percent = durationTimer / duration;
        UpdateFlags updateFlags = this.updateFlags;
        if (lifeValue.Independent) { GenerateLifeValues(); }
        if (lifeOffsetValue.Independent) { GenerateLifeOffsetValues(); }
        particle.currentLife = particle.life = life + (int)(lifeDiff * lifeValue.GetScale(percent));
        if (velocityValue.Active)
        {
            particle.velocity = velocityValue.NewLowValue();
            particle.velocityDiff = velocityValue.NewHighValue();
            if (!velocityValue.Relative) { particle.velocityDiff -= particle.velocity; }
        }
        particle.angle = angleValue.NewLowValue();
        particle.angleDiff = angleValue.NewHighValue();
        if (!angleValue.Relative) { particle.angleDiff -= particle.angle; }
        float angle = 0;
        if ((updateFlags & UpdateFlags.Angle) == 0)
        {
            angle = particle.angle + particle.angleDiff * angleValue.GetScale(0);
            particle.angle = angle;
            particle.angleCos = MathUtils.CosDeg(angle);
            particle.angleSin = MathUtils.SinDeg(angle);
        }
        float spriteWidth = sprite.Width;
        float spriteHeight = sprite.Height;
        particle.xScale = xScaleValue.NewLowValue() / spriteWidth;
        particle.xScaleDiff = xScaleValue.NewHighValue() / spriteWidth;
        if (!xScaleValue.Relative) { particle.xScaleDiff -= particle.xScale; }

        if (yScaleValue.Active)
        {
            particle.yScale = yScaleValue.NewLowValue() / spriteHeight;
            particle.yScaleDiff = yScaleValue.NewHighValue() / spriteHeight;
            if (!yScaleValue.Relative) { particle.yScaleDiff -= particle.yScale; }
            //particle.SetScale()
        }
        else
        {
            //particle.SetScale();
        }
        if (rotationValue.Active)
        {
            particle.rotation = rotationValue.NewLowValue();
            particle.rotationDiff = rotationValue.NewHighValue();
            if (!rotationValue.Relative) { particle.rotationDiff -= particle.rotation; }

        }

        if (windValue.Active)
        {
            particle.wind = windValue.NewLowValue();
            particle.windDiff = windValue.NewHighValue();
            if (!windValue.Relative) { particle.windDiff -= particle.wind; }
        }
        if (gravityValue.Active)
        {
            particle.gravity = gravityValue.NewLowValue();
            particle.gravityDiff = gravityValue.NewHighValue();
            if (!gravityValue.Relative) { particle.gravityDiff -= particle.gravity; }
        }

        particle.transparency = transparenyValue.NewLowValue();
        particle.transparencyDiff = transparenyValue.NewHighValue() - particle.transparency;

    }
    private void GenerateLifeValues()
    {
        life = (int)lifeValue.NewLowValue();
        lifeDiff = (int)lifeValue.NewHighValue();
        if (!lifeValue.Relative) { lifeDiff -= life; }
    }
    private void GenerateLifeOffsetValues()
    {
        lifeOffset = lifeOffsetValue.Active ? (int)lifeOffsetValue.NewLowValue() : 0;
        lifeOffsetDiff = (int)lifeOffsetValue.NewHighValue();
        if (!lifeOffsetValue.Relative) { lifeOffsetDiff -= lifeOffset; }
    }

    public void SetPosition(float x, float y)
    {
        if (attached)
        {
            float xAmount = x - this.x;
            float yAmount = y - this.y;
            bool[] active = this.active;
            for (int i = 0, n = active.Length; i < n; i++)
            {
                if (active[i]) { particles[i].Translate(xAmount, yAmount); }
            }

        }
        this.x = x;
        this.y = y;
    }

    public bool Attached
    {
        get => attached;
        set => attached = value;
    }

    public bool Continuous
    {
        get => continuous;
        set => continuous = value;
    }

    public bool Additive
    {
        get => additive;
        set => additive = value;
    }

    public bool CleansUpBlendFunction
    {
        get => cleansUpBlendFunction;
        set => cleansUpBlendFunction = value;
    }

    public bool Behind
    {
        get => behind;
        set => behind = value;
    }

    public bool PremultipliedAlpha
    {
        get => premultipliedAlpha;
        set => premultipliedAlpha = value;
    }

    public int MinParticleCount
    {
        get => minParticleCount;
        set => minParticleCount = value;
    }

    public bool IsComplete
    {
        get
        {
            if (continuous && !allowCompletion) return false;
            if (delayTimer < delay) return false;
            return durationTimer >= duration && activeCount == 0;
        }
    }

    public float PercentComplete
    {
        get
        {
            if (delayTimer < delay) return 0;
            return MathF.Min(1, durationTimer / duration);
        }
    }

    public float X => x;
    public float Y => y;
    private class ParticleValue
    {
        private bool active;
        private bool alwaysActive;

        public bool AlwaysActive
        {
            get => alwaysActive;
            set => alwaysActive = value;
        }

        public bool Active
        {
            get => alwaysActive || active;
            set { active = value; }
        }
    }

    private class NumericValue : ParticleValue
    {
        private float value;

        public float Value
        {
            get => value;
            set => this.value = value;
        }
    }

    private class RangedNumericValue : ParticleValue
    {
        private float lowMin;
        private float lowMax;

        public float NewLowValue()
        {
            return lowMin + (lowMax - lowMin) * rng.NextSingle();
        }
        public float LowMin
        {
            get => lowMin;
            set => lowMin = value;
        }

        public float LowMax
        {
            get => lowMax;
            set => lowMax = value;
        }
    }

    private class ScaledNumericValue : RangedNumericValue
    {
        private float[] scaling = new float[] { 1.0f };
        internal float[] timeline = new float[] { 0.0f };
        private float highMin;
        private float highMax;
        private bool relative;

        public float NewHighValue()
        {
            return highMin + (highMax - highMin) * rng.NextSingle();
        }

        public bool Relative
        {
            get => relative;
            set => relative = value;
        }

        public float HighMin
        {
            get => highMin;
            set => highMin = value;
        }

        public float HighMax
        {
            get => highMax;
            set => highMax = value;
        }

        public float GetScale(float percent)
        {
            int endIndex = -1;
            float[] timeline = this.timeline;
            int n = timeline.Length;
            for (int i = 1; i < n; i++)
            {
                float t = timeline[i];
                if (t > percent)
                {
                    endIndex = i;
                    break;
                }
            }
            if (endIndex == -1) { return this.scaling[n - 1]; }
            float[] scaling = this.scaling;
            int startIndex = endIndex - 1;
            float startValue = scaling[startIndex];
            float startTime = timeline[startIndex];
            return startValue + (scaling[endIndex] - startValue) * ((percent - startTime) / (timeline[endIndex] - startTime));
        }
    }

    private class IndependentScaledNumericValue : ScaledNumericValue
    {
        private bool independent;

        public bool Independent
        {
            get => independent;
            set => independent = value;
        }
    }

    public class Particle : TextureRegion
    {
        public int life;
        public int currentLife;
        public float xScale;
        public float xScaleDiff;
        public float yScale;
        public float yScaleDiff;
        public float rotation;
        public float rotationDiff;
        public float velocity;
        public float velocityDiff;
        public float angle;
        public float angleDiff;
        public float angleCos;
        public float angleSin;
        public float transparency;
        public float transparencyDiff;
        public float wind;
        public float windDiff;
        public float gravity;
        public float gravityDiff;

        public int frame;
        public Particle(TextureRegion sprite)
            : base(sprite)
        {

        }
        internal void Draw(IRenderer renderer)
        {
        }

        internal void Translate(float xAmount, float yAmount)
        {
        }
    }
}
