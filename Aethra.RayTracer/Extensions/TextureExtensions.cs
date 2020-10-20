using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Basic.Textures;

namespace Aethra.RayTracer.Extensions
{
    public static class TextureExtensions
    {
        public static TextureInfo ToInfo(this ITexture texture) => new TextureInfo(texture);
        public static TextureInfo ToInfo(this ITexture texture, float scale) => new TextureInfo(texture, scale, Vector2.Zero);
    }
}