using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Cameras
{
    public class PinholeCamera : Camera
    {
        public readonly Vector2 Scale;
        public readonly float Distance;

        public PinholeCamera(Framebuffer renderTarget, Vector3 position, Vector3 direction, Vector3 up, Vector2 scale,
            float distance) : base(renderTarget, position, direction, up)
        {
            Scale = scale;
            Distance = distance;
            W = Position - Direction;
            W = W.Normalize();
            U = Up.Cross(W);
            U = U.Normalize();
            V = W.Cross(U);
        }

        protected override Ray CreateRay(float x, float y)
        {
            (x,y) = new Vector2(x / RenderTarget.Width * 2 - 1, y / RenderTarget.Height * 2 - 1);
            var vpLoc = new Vector2(x * Scale.X, y * Scale.Y);
            return new Ray(Position, RayDirection(vpLoc).Normalize());
        }

        private Vector3 RayDirection(Vector2 v)
        {
            var (x, y, z) = new Vector3(v.X, v.Y, -Distance);
            return U * x + V * y + W * z;
        }
    }
}