using System;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Interfaces;

namespace Aethra.RayTracer.Primitives
{
    public readonly struct Sphere : IHittable
    {
        public readonly Vector3 Center;
        public readonly float Radius;
        public Material Material { get; }

        public Sphere(Vector3 center = default, float radius = default, Material? material = default)
        {
            Center = center;
            Radius = radius;
            Material = material ?? Material.Error;
        }
        
        public bool Hit(Ray ray, out RayHit hit)
        {
            var center = Center;
            var oc = ray.Origin - center;
            var rayDir = ray.Direction;
            var b = Vector3.Dot(oc, rayDir);
            var radius = Radius;
            var c = Vector3.Dot(oc, oc) - radius * radius;
            var discriminant = b * b - c;
        
            if (discriminant > 0)
            {
                var tmp = MathF.Sqrt(discriminant);
                var distance = -b - tmp;
                if (ray.IsInsidePlanes(distance))
                {
                    var position = ray.PointAt(distance);
                    var normal = (position - center) / radius;
                    hit = new RayHit(position, distance, normal, Material, GetTextureCoords(normal));
                    return true;
                }
        
                distance = -b + tmp;
                if (ray.IsInsidePlanes(distance))
                {
                    var position = ray.PointAt(distance);
                    var normal = (position - center) / radius;
                    hit = new RayHit(position, distance, normal, Material, GetTextureCoords(normal));
                    return true;
                }
            }
        
            hit.Position = Vector3.Zero;
            hit.Normal = Vector3.Zero;
            hit.Distance = 0;
            hit.Material = Material.Error;
            hit.TextureCoords = default;
            hit.Depth = 0;
            return false;
        }

        private Vector2 GetTextureCoords(Vector3 normal)
        {
            var phi = MathF.Atan2(normal.X, normal.Z);
            var theta = MathF.Acos(normal.Y);
            if (phi < 0)
            {
                phi += 2 * MathF.PI;
            }

            var u = phi * (1 / (2 * MathF.PI));
            var v = 1 - theta * (1 / MathF.PI);
            return new Vector2(u, v);
        }
    }
}