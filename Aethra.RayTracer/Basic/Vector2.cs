using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aethra.RayTracer.Extensions;

namespace Aethra.RayTracer.Basic
{
    public readonly struct Vector2 : IEquatable<Vector2>, IFormattable
    {
        public readonly float X;
        public readonly float Y;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2(float x = default, float y = default)
        {
            X = x;
            Y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2(Vector2 v)
        {
            X = v.X;
            Y = v.Y;
        }
        
        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Vector2 Zero = new Vector2();
        public static readonly Vector2 One = new Vector2(1, 1);

        public static readonly Vector2 PositiveInfinity =
            new Vector2(float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Vector2 NegativeInfinity =
            new Vector2(float.NegativeInfinity, float.NegativeInfinity);

        public float Length => MathF.Sqrt(MathF.Pow(X, 2) + MathF.Pow(Y, 2));

        public float LengthSquared => MathF.Pow(X, 2) + MathF.Pow(Y, 2);

        #region Operators

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2 operator *(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.X * v2.X, v1.Y * v2.Y);
        }

        public static Vector2 operator *(Vector2 v, float scalar)
        {
            return new Vector2(v.X * scalar, v.Y * scalar);
        }

        public static Vector2 operator *(float scalar, Vector2 v)
        {
            return new Vector2(v.X * scalar, v.Y * scalar);
        }

        public static Vector2 operator /(Vector2 v, float scalar)
        {
            var inverse = 1f / scalar;
            return new Vector2(v.X * inverse, v.Y * inverse);
        }

        public static Vector2 operator /(float scalar, Vector2 v)
        {
            return new Vector2(scalar / v.X, scalar / v.Y);
        }

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }

        #endregion

        #region Equality

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Vector2 other)
        {
            var diffX = X - other.X;
            var diffY = Y - other.Y;
            var sqrMag = diffX * diffX + diffY * diffY;
            return sqrMag < DoubledEpsilon;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)})";
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        #endregion
        
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, float t)
        {
            return new Vector2(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t
            );
        }

        public Vector2 Normalize()
        {
            var length = Length;
            if (length > Epsilon)
            {
                return this / length;
            }

            return Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(Vector2 v)
        {
            return X * v.X + Y * v.Y;
        }

        public float Cross(Vector2 v)
        {
            return X * v.Y - Y * v.X;
        }

        public Vector2 Reflect(Vector2 normal)
        {
            // return this - 2 * Dot(normal) * normal;
            //return 2 * normal * Dot(normal) - this;
            var factor = -2F * Dot(normal);
            return new Vector2(factor * normal.X + X, factor * normal.Y + Y);
            //return 2 * normal * normal.Dot(this) - this;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 Lerp(Vector2 v, float t)
        {
            return new Vector2(X + t * (v.X - X), Y + t * (v.Y - Y));
        }

        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;
        private const float EpsilonNormalSqrt = 1e-15f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector2 v1, Vector2 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y;
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            // sqrt(a) * sqrt(b) = sqrt(a * b) -- valid for real numbers
            var denominator = (float) Math.Sqrt(from.LengthSquared * to.LengthSquared);
            if (denominator < EpsilonNormalSqrt)
            {
                return 0f;
            }

            var dot = MathExtensions.Clamp(Dot(from, to) / denominator, -1f, 1f);
            return (float) Math.Acos(dot) * MathExtensions.Rad2Deg;
        }

        public static float Distance(Vector2 v1, Vector2 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            return MathF.Sqrt(diffX * diffX + diffY * diffY);
        }


        private System.Numerics.Vector2 ToBuiltIn()
        {
            return new System.Numerics.Vector2(X, Y);
        }

        private static Vector2 FromBuiltIn(System.Numerics.Vector2 vector2)
        {
            return new Vector2(vector2.X, vector2.Y);
        }

        public static Vector2 Transform(Vector2 vector3, float scale, Vector2 position, Vector2 rotationAxis, float angle)
        {
            var matrixScale = Matrix3x2.CreateScale(scale);
            var matrixPosition = Matrix3x2.CreateTranslation(position.ToBuiltIn());
            var matrixRotation = Matrix3x2.CreateRotation(angle * MathExtensions.Deg2Rad,rotationAxis.ToBuiltIn());
            var transformMatrix = matrixScale * matrixRotation * matrixPosition;
            return FromBuiltIn(System.Numerics.Vector2.Transform(vector3.ToBuiltIn(), transformMatrix));
        }


        public static implicit operator Vector2((float x, float y, float z) values) =>
            new Vector2(values.x, values.y);

        public static implicit operator (float x, float y)(Vector2 v) => (v.X, v.Y);

        public void Deconstruct(out float x, out float y) => (x, y) = (X, Y);
    }
}