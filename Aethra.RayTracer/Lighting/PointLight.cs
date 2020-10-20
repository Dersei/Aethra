using System;
using System.Drawing;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Lighting
{
    public class PointLight : Light
    {
        public float Radius { get; set; } = 1;

        public override Vector3 GetDirection(RayHit hitInfo)
        {
            var hitPosition = hitInfo.Position;
            if (Sampler is null) return (Position - hitPosition).Normalize();
            if (Radius.IsAboutZero())
            {
                return (Position - hitPosition).Normalize();
            }

            var sample = Sampler.Single();
            return (Position + RemapSampleToUnitSphere(sample) * Radius - hitPosition).Normalize();
        }

        public override FloatColor GetLightColor(Vector3 hitPosition)
        {
            if (Radius.IsAboutZero()) return Color;
            return Color / Vector3.Distance(hitPosition, Position) * Radius;
        }

        private static Vector3 RemapSampleToUnitSphere(Vector2 sample)
        {
            var z = 2 * sample.X - 1;
            var t = 2 * MathF.PI * sample.Y;
            var r = MathF.Sqrt(1 - z * z);
            return new Vector3(r * MathF.Cos(t), r * MathF.Sin(t), z);
        }
    }
}