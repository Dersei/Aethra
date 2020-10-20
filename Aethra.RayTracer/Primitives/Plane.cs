using System;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Interfaces;

namespace Aethra.RayTracer.Primitives
{
    public readonly struct Plane : IHittable
    {
        public readonly Vector3 Normal;
        public readonly Vector3 Point;
        public Material Material { get; }

        public Plane(Vector3 point = default, Vector3 normal = default, Material? material = default)
        {
            Normal = normal;
            Point = point;
            Material = material ?? Material.Error;
        }

        public bool Hit(Ray ray, out RayHit hit)
        {
            var discriminant = Vector3.Dot(Normal, ray.Direction);
            hit = new RayHit();
            if (discriminant.IsAboutZero()) return false;
            var direction = Point - ray.Origin;
            var distance = Vector3.Dot(direction, Normal) / discriminant;
            if (distance < 0) return false;
            if (!ray.IsInsidePlanes(distance)) return false;
            hit.Distance = distance;
            hit.Normal = Normal;
            hit.Position = ray.PointAt(distance);
            hit.Material = Material;
            hit.TextureCoords = GetTextureCoords(hit.Position);
            return true;
        }

        private Vector2 GetTextureCoords(Vector3 position)
        {
            var e1 = Normal.Cross(Vector3.Right);

            if (e1 == Vector3.Zero)
            {
                e1 = Normal.Cross(Vector3.Forward);
            }

            var e2 = Normal.Cross(e1).Normalize();
            var u = MathF.Abs(Vector3.Dot(e1, position));
            var v = MathF.Abs(Vector3.Dot(e2, position));
            return new Vector2(u, v);
        }
    }
}