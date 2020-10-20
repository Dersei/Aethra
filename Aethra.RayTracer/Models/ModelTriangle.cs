using System.Runtime.CompilerServices;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Interfaces;

namespace Aethra.RayTracer.Models
{
    public readonly struct ModelTriangle : IHittable
    {
        public readonly Vector3 Normal;
        public readonly FaceVertex FV1;
        public readonly FaceVertex FV2;
        public readonly FaceVertex FV3;

        public Material Material { get; }

        public ModelTriangle(FaceVertex fv1 = default,
            FaceVertex fv2 = default, FaceVertex fv3 = default, Material? material = default)
        {
            FV1 = fv1;
            FV2 = fv2;
            FV3 = fv3;
            Material = material ?? Material.Error;
            Normal = (FV2.Position - FV1.Position).Cross(FV3.Position - FV1.Position).Normalize();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float CheckVertex(Vector3 a, Vector3 b, Vector3 c, Vector3 n) => (b - a).Cross(c - a).Dot(n);
        
        public bool Hit(Ray ray, out RayHit hit)
        {
            var discriminant = Vector3.Dot(Normal, ray.Direction);
            hit = new RayHit();
            if (discriminant.IsAboutZero()) return false;
            var direction = FV1.Position - ray.Origin;
            var distance = Vector3.Dot(direction, Normal) / discriminant;
            if (distance < 0) return false;
            if (!ray.IsInsidePlanes(distance)) return false;
            hit.Distance = distance;
            hit.Normal = Normal;
            hit.Position = ray.PointAt(distance);
            hit.Material = Material;

            var o1 = CheckVertex(hit.Position, FV1.Position, FV2.Position, Normal);
            var o2 = CheckVertex(hit.Position, FV2.Position, FV3.Position, Normal);
            var o3 = CheckVertex(hit.Position, FV3.Position, FV1.Position, Normal);

            var rayCross = ray.Direction.Cross(FV3.Position - FV1.Position);
            var rayDot = (FV2.Position - FV1.Position).Dot(rayCross);
            var invertedRayDot = 1 / rayDot;
            var u = (-direction).Dot(rayCross) * invertedRayDot;
            var directionCross = (-direction).Cross(FV2.Position - FV1.Position);
            var v = ray.Direction.Dot(directionCross) * invertedRayDot;
            hit.TextureCoords = (1 - u - v) * FV1.TextureCoords + u * FV2.TextureCoords + v * FV3.TextureCoords;
            return o1 > 0 && o2 > 0 && o3 > 0;
        }
    }
}