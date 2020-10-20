using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Basic.Materials
{
    public abstract class Material
    {
        public FloatColor Color;
        public TextureInfo? Texture;

        protected Material()
        {
            
        }

        protected Material(FloatColor color, TextureInfo? texture = null)
        {
            Color = color;
            Texture = texture;
        }
        
        public abstract FloatColor CalculateColor(Scene scene, Ray ray, RayHit hit);
        
        public static readonly Material Error = new EmissiveMaterial(FloatColor.Error, 100);
    }
}