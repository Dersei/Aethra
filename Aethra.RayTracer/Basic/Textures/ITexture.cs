namespace Aethra.RayTracer.Basic.Textures
{
    public interface ITexture
    {
        public FloatColor GetColor(Vector2 uv, Vector2 scale, Vector2 offset) => FloatColor.Error;
        public FloatColor GetColor(Vector2 uv) => FloatColor.Error;
    }
}