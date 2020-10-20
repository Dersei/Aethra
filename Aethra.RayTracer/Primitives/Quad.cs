using System.Runtime.CompilerServices;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Interfaces;

namespace Aethra.RayTracer.Primitives
{
    public readonly struct Quad : IHittable
    {
        public readonly Vector3 Q1;
        public readonly Vector3 Q2;
        public readonly Vector3 Q3;
        public readonly Vector3 Q4;
        public readonly Vector3 Normal;
        public Material Material { get; }

        public Quad(Vector3 q1, Vector3 q2, Vector3 q3, Vector3 q4, FloatColor color = default)
        {
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
            Q4 = q4;
            Material = new PhongMaterial(color);
            Normal = (Q2 - Q1).Cross(Q3 - Q1).Normalize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CheckVertex(Vector3 a, Vector3 b, Vector3 c, Vector3 n) => (b - a).Cross(c - a).Dot(n);

        public bool Hit(Ray ray, out RayHit hit)
        {
            var test = true;
            var discriminant = Vector3.Dot(Normal, ray.Direction);
            hit = new RayHit();
            if (discriminant.IsAboutZero()) test = false;
            var direction = Q1 - ray.Origin;
            var distance = Vector3.Dot(direction, Normal) / discriminant;
            if (distance < 0) test = false;
            if (!ray.IsInsidePlanes(distance)) test = false;
            hit.Distance = distance;
            hit.Normal = Normal;
            hit.Position = ray.PointAt(distance);
            hit.Material = Material;

            if (test)
            {
                var o1 = CheckVertex(hit.Position, Q1, Q2, Normal);
                var o2 = CheckVertex(hit.Position, Q2, Q3, Normal);
                var o3 = CheckVertex(hit.Position, Q3, Q4, Normal);
                var o4 = CheckVertex(hit.Position, Q4, Q1, Normal);
                return o1 > 0 && o2 > 0 && o3 > 0 && o4 > 0;
            }

            return false;
        }
    }
}