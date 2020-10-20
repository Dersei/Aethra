using System;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Basic.Textures;
using Aethra.RayTracer.Interfaces;

namespace Aethra.RayTracer.Primitives
{
    public readonly struct DistortedSphere2 : IHittable
    {
        public readonly Vector3 Center;
        public readonly float Radius;
        public readonly Texture DisplacementMap;
        public readonly float DisplacementScale;
        public Material Material { get; }

        public DistortedSphere2(Vector3 center, float radius, Texture displacementMap, float displacementScale,
            Material? material = default)
        {
            Center = center;
            Radius = radius;
            Material = material ?? Material.Error;
            DisplacementMap = displacementMap;
            DisplacementScale = displacementScale;
        }

        private static Random _random = new Random();

        private bool GetStandardHit(Ray ray, out RayHit hit)
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
                    var normal = (position - center).Normalize();
                    hit = new RayHit(position, distance, normal, Material, GetTextureCoords(position - Center));
                    return true;
                }

                distance = -b + tmp;
                if (ray.IsInsidePlanes(distance))
                {
                    var position = ray.PointAt(distance);
                    var normal = (position - center).Normalize();
                    hit = new RayHit(position, distance, normal, Material, GetTextureCoords(position - Center));
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
        
        public bool Hit(Ray ray, out RayHit hit)
        {
            if (!GetStandardHit(ray, out var standardHit))
            {
                hit.Position = Vector3.Zero;
                hit.Normal = Vector3.Zero;
                hit.Distance = 0;
                hit.Material = Material.Error;
                hit.TextureCoords = default;
                hit.Depth = 0;
                return false;
            }
            var disp = UpdatePosition(standardHit.Position, standardHit.Normal);
            var center = Center;
            //var oc = ray.Origin - center/disp.X;
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
                    var normal = (position - center).Normalize();
                    hit = new RayHit(position, distance, normal, Material, GetTextureCoords(position - Center));
                    return true;
                }

                distance = -b + tmp;
                if (ray.IsInsidePlanes(distance))
                {
                    var position = ray.PointAt(distance);
                    var normal = (position - center).Normalize();
                    hit = new RayHit(position, distance, normal, Material, GetTextureCoords(position - Center));
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

        private Vector3 UpdatePosition(Vector3 position, Vector3 normal)
        {
            var phi = MathF.Atan2(position.X, position.Z);
            var theta = MathF.Acos(position.Y);
            if (phi < 0)
            {
                phi += 2 * MathF.PI;
            }

            var u = phi * (1 / (2 * MathF.PI));
            var v = 1 - theta * (1 / MathF.PI);
            var value = DisplacementMap.GetColor(new Vector2(u, v)) * 2 - FloatColor.White;
            var displacementVector = new Vector3(value.R, value.G, value.B);
            var displacement = new Vector3(0.21f, 0.72f, 0.07f).Dot(displacementVector);
            return (position - Center).Normalize() * displacement * DisplacementScale;
        }

        private Vector2 GetTextureCoords(Vector3 position)
        {
            var phi = MathF.Atan2(position.X, position.Z);
            var theta = MathF.Acos(position.Y);
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