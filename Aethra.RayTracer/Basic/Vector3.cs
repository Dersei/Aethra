using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Aethra.RayTracer.Extensions;

namespace Aethra.RayTracer.Basic
{
    public readonly struct Vector3 : IEquatable<Vector3>, IFormattable
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
        }

        public float this[int index] => index switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new ArgumentOutOfRangeException(nameof(index))
        };

        public static readonly Vector3 Zero = new Vector3();
        public static readonly Vector3 One = new Vector3(1, 1, 1);
        public static readonly Vector3 Up = new Vector3(0f, 1f, 0);
        public static readonly Vector3 Down = new Vector3(0f, -1f, 0);
        public static readonly Vector3 Left = new Vector3(-1f, 0, 0);
        public static readonly Vector3 Right = new Vector3(1f, 0, 0);
        public static readonly Vector3 Forward = new Vector3(0f, 0f, 1f);
        public static readonly Vector3 Back = new Vector3(0f, 0f, -1f);

        public static readonly Vector3 PositiveInfinity =
            new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

        public static readonly Vector3 NegativeInfinity =
            new Vector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

        public float Length => MathF.Sqrt(MathF.Pow(X, 2) + MathF.Pow(Y, 2) + MathF.Pow(Z, 2));

        public float LengthSquared => MathF.Pow(X, 2) + MathF.Pow(Y, 2) + MathF.Pow(Z, 2);

        #region Operators

        public static Vector3 operator +(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3 operator -(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3 operator *(Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }

        public static Vector3 operator *(Vector3 v, float scalar)
        {
            return new Vector3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Vector3 operator *(float scalar, Vector3 v)
        {
            return new Vector3(v.X * scalar, v.Y * scalar, v.Z * scalar);
        }

        public static Vector3 operator /(Vector3 v, float scalar)
        {
            var inverse = 1f / scalar;
            return new Vector3(v.X * inverse, v.Y * inverse, v.Z * inverse);
        }

        public static Vector3 operator /(float scalar, Vector3 v)
        {
            return new Vector3(scalar / v.X, scalar / v.Y, scalar / v.Z);
        }

        public static Vector3 operator -(Vector3 v)
        {
            return new Vector3(-v.X, -v.Y, -v.Z);
        }

        #endregion

        #region Equality

        public static bool operator ==(Vector3 left, Vector3 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector3 left, Vector3 right)
        {
            return !left.Equals(right);
        }

        public bool Equals(Vector3 other)
        {
            var diffX = X - other.X;
            var diffY = Y - other.Y;
            var diffZ = Z - other.Z;
            var sqrMag = diffX * diffX + diffY * diffY + diffZ * diffZ;
            return sqrMag < DoubledEpsilon;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector3 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            return
                $"({X.ToString(format, formatProvider)}, {Y.ToString(format, formatProvider)}, {Z.ToString(format, formatProvider)})";
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        #endregion

        public Vector3 Normalize()
        {
            var length = Length;
            if (length > Epsilon)
            {
                return this / length;
            }

            return Zero;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float Dot(Vector3 v)
        {
            return X * v.X + Y * v.Y + Z * v.Z;
        }

        public Vector3 Cross(Vector3 v)
        {
            return new Vector3(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);
        }

        public Vector3 Reflect(Vector3 normal)
        {
            // return this - 2 * Dot(normal) * normal;
            //return 2 * normal * Dot(normal) - this;
            return 2 * normal * normal.Dot(this) - this;
        }
        
        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            float sqrMag = Dot(onNormal, onNormal);
            if (sqrMag < Epsilon)
                return Zero;
            else
            {
                var dot = Dot(vector, onNormal);
                return new Vector3(onNormal.X * dot / sqrMag,
                    onNormal.Y * dot / sqrMag,
                    onNormal.Z * dot / sqrMag);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 Lerp(Vector3 v, float t)
        {
            return new Vector3(X + t * (v.X - X), Y + t * (v.Y - Y), Z + t * (v.Z - Z));
        }

        private const float Epsilon = 0.00001f;
        private const float DoubledEpsilon = Epsilon * Epsilon;
        private const float EpsilonNormalSqrt = 1e-15f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static float Angle(Vector3 from, Vector3 to)
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

        public static float Distance(Vector3 v1, Vector3 v2)
        {
            var diffX = v1.X - v2.X;
            var diffY = v1.Y - v2.Y;
            var diffZ = v1.Z - v2.Z;
            return MathF.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);
        }
        
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = MathExtensions.Clamp01(t);
            return LerpUnclamped(a, b, t);
        }

        // Linearly interpolates between two vectors without clamping the interpolant
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            return new Vector3(
                a.X + (b.X - a.X) * t,
                a.Y + (b.Y - a.Y) * t,
                a.Z + (b.Z - a.Z) * t
            );
        }


        private System.Numerics.Vector3 ToBuiltIn()
        {
            return new System.Numerics.Vector3(X, Y, Z);
        }

        private static Vector3 FromBuiltIn(System.Numerics.Vector3 vector3)
        {
            return new Vector3(vector3.X, vector3.Y, vector3.Z);
        }

        public static Vector3 Transform(Vector3 vector3, float scale, Vector3 position, Vector3 rotationAxis,
            float angle)
        {
            var matrixScale = Matrix4x4.CreateScale(scale);
            var matrixPosition = Matrix4x4.CreateTranslation(position.ToBuiltIn());
            var matrixRotation =
                Matrix4x4.CreateFromAxisAngle(rotationAxis.ToBuiltIn(), angle * MathExtensions.Deg2Rad);
            var transformMatrix = matrixScale * matrixRotation * matrixPosition;
            return FromBuiltIn(System.Numerics.Vector3.Transform(vector3.ToBuiltIn(), transformMatrix));
        }


        public static implicit operator Vector3((float x, float y, float z) values) =>
            new Vector3(values.x, values.y, values.z);

        public static implicit operator (float x, float y, float z)(Vector3 v) => (v.X, v.Y, v.Z);

        public void Deconstruct(out float x, out float y, out float z) => (x, y, z) = (X, Y, Z);
    }
}