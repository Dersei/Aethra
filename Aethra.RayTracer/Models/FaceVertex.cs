using Aethra.RayTracer.Basic;

namespace Aethra.RayTracer.Models
{
    public readonly struct FaceVertex
    {
        public readonly Vector3 Position;
        public readonly Vector3 Normal;
        public readonly Vector2 TextureCoords;

        public FaceVertex(Vector3 pos, Vector3 norm, Vector2 texCoords)
        {
            Position = pos;
            Normal = norm;
            TextureCoords = texCoords;
        }
    }
}