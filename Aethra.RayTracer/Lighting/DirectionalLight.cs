using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Lighting
{
    public class DirectionalLight : Light
    {
        private Vector3 _direction;

        public Vector3 Direction
        {
            get => _direction;
            set
            {
                _direction = value;
                Position = -_direction * 1000_000f;
            }
        }

        public override Vector3 GetDirection(RayHit hitInfo)
        {
            return -_direction;
        }

        public override FloatColor GetLightColor(Vector3 hitPosition)
        {
            return Color;
        }
    }
}