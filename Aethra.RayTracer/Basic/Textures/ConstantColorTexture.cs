namespace Aethra.RayTracer.Basic.Textures
{
    public class ConstantColorTexture : ITexture
    {
        public readonly FloatColor Color;

        public ConstantColorTexture(FloatColor color) => Color = color;
        
        public FloatColor GetColor(Vector2 uv) => Color;

        public FloatColor GetColor(Vector2 uv, Vector2 scale, Vector2 offset) => GetColor(uv);
    }
}