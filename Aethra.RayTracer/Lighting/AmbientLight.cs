using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Lighting
{
    public class AmbientLight : Light
    {
        public override Vector3 GetDirection(RayHit hitInfo)
        {
            Position = hitInfo.Position;
            return hitInfo.Normal;
        }

        public override FloatColor GetLightColor(Vector3 hitPosition)
        {
            return Color;
        }
    }
}