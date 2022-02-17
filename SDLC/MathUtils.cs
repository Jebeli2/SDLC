// Copyright © 2021 Jean Pascal Bellot. All Rights Reserved.
// Licensed under the GNU General Public License.

namespace SDLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class MathUtils
{

    public static readonly float Phi = (MathF.Sqrt(5.0f) + 1.0f) / 2.0f;
    public static float Mix(int a, int b, float amount)
    {
        return (1 - amount) * a + amount * b;
    }

    public static float Mix(float a, float b, float amount)
    {
        return (1 - amount) * a + amount * b;
    }

    public static double Mix(double a, double b, double amount)
    {
        return (1 - amount) * a + amount * b;
    }

    public static float Clamp(float a, float min, float max)
    {
        if (a > max) a = max;
        if (a < min) a = min;
        return a;
    }
    public static int Clamp(int a, int min, int max)
    {
        if (a > max) a = max;
        if (a < min) a = min;
        return a;
    }
    public static double Clamp(double a, double min, double max)
    {
        if (a > max) a = max;
        if (a < min) a = min;
        return a;
    }

    public static float Deg2Rad(float deg)
    {
        return MathF.PI * deg / 180.0f;
    }
    public static float CosDeg(float angle)
    {
        return MathF.Cos(Deg2Rad(angle));
    }

    public static float SinDeg(float angle)
    {
        return MathF.Sin(Deg2Rad(angle));
    }
}
