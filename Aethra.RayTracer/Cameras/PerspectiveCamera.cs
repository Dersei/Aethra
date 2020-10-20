using System;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Extensions;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Cameras
{
    public class PerspectiveCamera : Camera
    {
        public float Fov { get; } = 60f;

        public PerspectiveCamera(Framebuffer renderTarget, Vector3 position, Vector3 direction, Vector3 up) : base(
            renderTarget, position,
            direction, up)
        {
        }

        protected override Ray CreateRay(float x, float y)
        {
            var (midX, midY) = GetCenter(x, y);
            midX *= MathF.Tan(Fov * MathExtensions.Deg2Rad);
            midY *= MathF.Tan(Fov * MathExtensions.Deg2Rad);
            return new Ray(Position, (-5f * W + midX * U + midY * V).Normalize(), NearPlane, FarPlane);
        }
    }
}