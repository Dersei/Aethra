using System;

namespace Aethra.RayTracer.Extensions
{
    public static class MathExtensions
    {
        public const float Deg2Rad = MathF.PI * 2F / 360F;
        public const float Rad2Deg = 1F / Deg2Rad;

        public static float Clamp(float value, float min, float max)
        {
            if (value <= min)
                return min;
            return value >= max ? max : value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value <= min)
                return min;
            return value >= max ? max : value;
        }

        public static byte ClampToByte(this int value)
        {
            if (value <= 0)
                value = 0;
            else if (value >= 255)
                value = 255;
            return (byte) value;
        }

        public static byte ClampToByte(this float value)
        {
            return value <= 0 ? (byte) 0 : value >= 255 ? (byte) 255 : (byte) value;
            // if (value < 0)
            //    return 0;
            // else if (value > 255)
            //     return 255;
            // return (byte)value;
        }

        public static byte ClampToByte(this double value)
        {
            if (value <= 0)
                value = 0;
            else if (value >= 255)
                value = 255;
            return (byte) value;
        }

        public static float Clamp01(float value)
        {
            if (value <= 0F)
                return 0F;
            return value >= 1F ? 1F : value;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * Clamp01(t);
        }

        public static bool IsAbout(this float first, float second, float precisionMultiplier = 8)
        {
            return MathF.Abs(second - first) < MathF.Max(1E-06f * MathF.Max(MathF.Abs(first), MathF.Abs(second)),
                float.Epsilon * precisionMultiplier);
        }

        public static bool IsNotZero(this float value) => MathF.Abs(value) > float.Epsilon;

        public static bool IsAboutZero(this float value) => MathF.Abs(value) < float.Epsilon;
    }
}