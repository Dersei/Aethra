using System.Runtime.CompilerServices;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Interfaces;

namespace Aethra.RayTracer.Primitives
{
    public readonly struct Triangle : IHittable
    {
        public readonly Vector3 Q1;
        public readonly Vector3 Q2;
        public readonly Vector3 Q3;
        public readonly Vector3 Normal;

        public Material Material { get; }

        public Triangle(Vector3 q1, Vector3 q2, Vector3 q3, Material? material = default)
        {
            Q1 = q1;
            Q2 = q2;
            Q3 = q3;
            Material = material ?? Material.Error;
            Normal = (Q2 - Q1).Cross(Q3 - Q1).Normalize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CheckVertex(Vector3 a, Vector3 b, Vector3 c, Vector3 n) => (b - a).Cross(c - a).Dot(n);

        public bool Hit(Ray ray, out RayHit hit)
        {
            var discriminant = Vector3.Dot(Normal, ray.Direction);
            hit = new RayHit();
            if (discriminant.IsAboutZero()) return false;
            var direction = Q1 - ray.Origin;
            var distance = Vector3.Dot(direction, Normal) / discriminant;
            if (distance < 0) return false;
            if (!ray.IsInsidePlanes(distance)) return false;
            hit.Distance = distance;
            hit.Normal = Normal;
            hit.Position = ray.PointAt(distance);
            hit.Material = Material;

            var o1 = CheckVertex(hit.Position, Q1, Q2, Normal);
            var o2 = CheckVertex(hit.Position, Q2, Q3, Normal);
            var o3 = CheckVertex(hit.Position, Q3, Q1, Normal);
            
            return o1 > 0 && o2 > 0 && o3 > 0;
        }
    }
}