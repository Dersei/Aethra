using System;
using System.Runtime.CompilerServices;
using Aethra.RayTracer.Extensions;

namespace Aethra.RayTracer.Basic
{
    public struct FloatColor : IEquatable<FloatColor>, IFormattable
    {
        public readonly float R;
        public readonly float G;
        public readonly float B;
        public readonly float A;

        public FloatColor(float r, float g, float b, float a = 1f)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }


        public static FloatColor operator +(FloatColor v1, FloatColor v2)
        {
            return new FloatColor(v1.R + v2.R, v1.G + v2.G, v1.B + v2.B, v1.A + v2.A);
        }

        public static FloatColor operator -(FloatColor v1, FloatColor v2)
        {
            return new FloatColor(v1.R - v2.R, v1.G - v2.G, v1.B - v2.B, v1.A - v2.A);
        }

        public static FloatColor operator *(FloatColor color, int value)
        {
            return new FloatColor(color.R * value, color.G * value, color.B * value, color.A * value);
        }

        public static FloatColor operator *(FloatColor color, float value)
        {
            return new FloatColor(color.R * value, color.G * value, color.B * value, color.A * value);
        }

        public static FloatColor operator /(FloatColor color, float value)
        {
            return new FloatColor(color.R / value, color.G / value, color.B / value, color.A / value);
        }

        public static FloatColor operator *(FloatColor color1, FloatColor color2)
        {
            var r = color1.R * color2.R;
            var g = color1.G * color2.G;
            var b = color1.B * color2.B;
            var a = color1.A * color2.A;
            return new FloatColor(r, g, b, a);
        }

        /// <summary>
        /// Creates a new color from the normal vector after converting its values.
        /// </summary>
        /// <param name="normal">Normal</param>
        /// <returns></returns>
        public static FloatColor FromNormal(Vector3 normal)
        {
            var converted = (normal + Vector3.One) / 2;
            return new FloatColor(converted.X, converted.Y, converted.Z);
        }

        /// <summary>
        /// Creates a new color from the vector.
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns></returns>
        public static FloatColor FromVector(Vector3 vector)
        {
            return new FloatColor(vector.X, vector.Y, vector.Z);
        }
        
        /// <summary>
        /// Creates a new color from the vector.
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <returns></returns>
        public static FloatColor FromVectorSafe(Vector3 vector)
        {
            vector = vector.Normalize();
            vector = new Vector3(MathExtensions.Clamp01(vector.X),MathExtensions.Clamp01(vector.Y), MathExtensions.Clamp01(vector.Z));
            return new FloatColor(vector.X, vector.Y, vector.Z);
        }

        public static FloatColor FromRgb(float r, float g, float b)
        {
            return new FloatColor(r, g, b);
        }

        public static FloatColor FromRgba(float r, float g, float b, float a = 1)
        {
            return new FloatColor(r, g, b, a);
        }

        public static FloatColor FromRgba(int r, int g, int b, int a = 1)
        {
            return new FloatColor(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint ClampValue(float value)
        {
            return value <= 0 ? 0 : value >= 255 ? 255 : (uint) value;
        }

        /// <summary>
        /// Converts <c>FloatColor</c> to <c>uint</c>
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ToUint()
        {
            var a = ClampValue(A * 255);
            var r = ClampValue(R * 255);
            var g = ClampValue(G * 255);
            var b = ClampValue(B * 255);
            return (a << 24) | (r << 16) | (g << 8) | b;
        }

        /// <summary>
        /// Converts <c>FloatColor</c> to <c>uint</c>
        /// </summary>
        /// <returns></returns>
        public static implicit operator uint(FloatColor color)
        {
            return color.ToUint();
        }

        /// <summary>
        /// Converts <c>System.Drawing.Color</c> to <c>FloatColor</c>
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static implicit operator System.Drawing.Color(FloatColor color)
        {
            var a = (byte) color.A * 255;
            var r = (byte) color.R * 255;
            var g = (byte) color.G * 255;
            var b = (byte) color.B * 255;
            return System.Drawing.Color.FromArgb(a, r, g, b);
        }

        public static FloatColor Black = new FloatColor(0, 0, 0, 1);
        public static FloatColor White = new FloatColor(1, 1, 1, 1);
        public static FloatColor Grey = new FloatColor(0.5f, 0.5f, 0.5f, 1);
        public static FloatColor Red = new FloatColor(1, 0, 0, 1);
        public static FloatColor Blue = new FloatColor(0, 0, 1, 1);
        public static FloatColor Green = new FloatColor(0, 1, 0, 1);
        public static FloatColor Yellow = new FloatColor(1, 1, 0, 1);
        public static FloatColor UnityYellow = new FloatColor(1, 235f / 255f, 4f / 255f, 1);
        public static FloatColor Cyan = new FloatColor(0, 1, 1, 1);
        public static FloatColor Magenta = new FloatColor(1, 0, 1, 1);
        public static FloatColor Purple = new FloatColor(1, 0, 1, 1);
        public static FloatColor Error = new FloatColor(1, 0, 0.5647059F, 1);
        public static FloatColor Clear = new FloatColor(0, 0, 0, 0);

        public float Grayscale => 0.299F * R + 0.587F * G + 0.114F * B;
        public float MaxColorComponent => MathF.Max(MathF.Max(R, G), B);


        public bool Equals(FloatColor other)
        {
            return R.IsAbout(other.R) && G.IsAbout(other.G) && B.IsAbout(other.B) && A.IsAbout(other.A);
        }

        public override bool Equals(object? obj)
        {
            return obj is FloatColor other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }

        public static bool operator ==(FloatColor left, FloatColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(FloatColor left, FloatColor right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"FloatColor - ({R}, {G}, {B}, {A})";
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"FloatColor - ({R.ToString(format, formatProvider)}, {G.ToString(format, formatProvider)}, {B.ToString(format, formatProvider)})";
        }

        public static FloatColor LerpUnclamped(FloatColor a, FloatColor b, float t)
        {
            return new FloatColor(
                a.R + (b.R - a.R) * t,
                a.G + (b.G - a.G) * t,
                a.B + (b.B - a.B) * t,
                a.A + (b.A - a.A) * t
            );
        }

        public static FloatColor Lerp(FloatColor a, FloatColor b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        public float this[int index] =>
            index switch
            {
                0 => R,
                1 => G,
                2 => B,
                3 => A,
                _ => throw new IndexOutOfRangeException("Invalid Color index(" + index + ")!")
            };

        // Convert a set of HSV values to an RGB Color.
        public static FloatColor FromHsv(float h, float s, float v, bool hdr)
        {
            if (s.IsAboutZero())
            {
                return new FloatColor(v, v, v);
            }

            if (v.IsAboutZero())
            {
                return new FloatColor(0, 0, 0);
            }

            //crazy hsv conversion

            var tS = s;
            var tV = v;
            var hToFloor = h * 6.0f;

            var temp = (int) MathF.Floor(hToFloor);
            var t = hToFloor - temp;
            var var1 = (tV) * (1 - tS);
            var var2 = tV * (1 - tS * t);
            var var3 = tV * (1 - tS * (1 - t));

            var result = temp switch
            {
                0 => new FloatColor(tV, var3, var1),
                1 => new FloatColor(var2, tV, var1),
                2 => new FloatColor(var1, tV, var3),
                3 => new FloatColor(var1, var2, tV),
                4 => new FloatColor(var3, var1, tV),
                5 => new FloatColor(tV, var1, var2),
                6 => new FloatColor(tV, var3, var1),
                -1 => new FloatColor(tV, var1, var2),
                _ => Error
            };


            if (!hdr)
            {
                var r = MathExtensions.Clamp(result.R, 0.0f, 1.0f);
                var g = MathExtensions.Clamp(result.G, 0.0f, 1.0f);
                var b = MathExtensions.Clamp(result.B, 0.0f, 1.0f);
                return new FloatColor(r, g, b);
            }

            return result;
        }

        public static FloatColor ConvertToSRgb(FloatColor value)
        {
            var r = value.R > 0.0031308f ? 1.055f * MathF.Pow(value.R, 1.0f / 2.4f) - 0.055f : 12.92f * value.R;
            var g = value.G > 0.0031308f ? 1.055f * MathF.Pow(value.G, 1.0f / 2.4f) - 0.055f : 12.92f * value.G;
            var b = value.B > 0.0031308f ? 1.055f * MathF.Pow(value.B, 1.0f / 2.4f) - 0.055f : 12.92f * value.B;
            return new FloatColor(r, g, b);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FloatColor ConvertToGamma(FloatColor value)
        {
            var r = MathF.Pow(value.R, 1.0f / 2.2f);
            var g = MathF.Pow(value.B, 1.0f / 2.2f);
            var b = MathF.Pow(value.G, 1.0f / 2.2f);
            return new FloatColor(r, g, b);
        }
    }
}