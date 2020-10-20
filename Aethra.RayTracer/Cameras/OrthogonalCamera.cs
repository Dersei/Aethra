using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Cameras
{
    public class OrthogonalCamera : Camera
    {
        public OrthogonalCamera(Framebuffer renderTarget, Vector3 position, Vector3 direction, Vector3 up) : base(renderTarget,
            position, direction, up) { }
        
        protected override Ray CreateRay(float x, float y)
        {
            var (midX, midY) = GetCenter((int) x, (int) y);

            return new Ray(Position + midX * U + midY * V, -W, NearPlane, FarPlane);
        }
    }
}