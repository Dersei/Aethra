using Aethra.RayTracer.Basic.Textures;

namespace Aethra.RayTracer.Basic.Materials
{
    public class TextureInfo
    {
        public readonly ITexture Texture;
        public readonly Vector2 Scale;
        public readonly Vector2 Offset;
        
        public FloatColor GetColor(Vector2 uv) => Texture.GetColor(uv, Scale, Offset);

        public TextureInfo(ITexture texture, Vector2 scale, Vector2 offset)
        {
            Texture = texture;
            Scale = scale;
            Offset = offset;
        }
        
        public TextureInfo(ITexture texture, float scale, Vector2 offset)
        {
            Texture = texture;
            Scale = new Vector2(scale, scale);
            Offset = offset;
        }
        
        public TextureInfo(ITexture texture)
        {
            Texture = texture;
            Scale = Vector2.One;
            Offset = Vector2.Zero;
        }
    }
}