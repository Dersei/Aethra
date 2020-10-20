using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Basic.Materials
{
    public class EmissiveMaterial : Material
    {
        private readonly float _emissiveCoefficient;
        
        public EmissiveMaterial(FloatColor color, float emissiveCoefficient, TextureInfo? texture = null)
        {
            Color = color;
            _emissiveCoefficient = emissiveCoefficient;
            Texture = texture;
        }
        public override FloatColor CalculateColor(Scene scene, Ray ray, RayHit hit)
        {
            var texelColor = Texture?.GetColor(hit.TextureCoords);
            if (-hit.Normal.Dot(ray.Direction) > 0)
            {
                if (texelColor != null)
                {
                    return Color * texelColor.Value * _emissiveCoefficient;
                }

                return Color * _emissiveCoefficient;
            }

            return FloatColor.Black;
        }
    }
}