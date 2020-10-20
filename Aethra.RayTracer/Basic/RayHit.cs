using Aethra.RayTracer.Basic.Materials;

namespace Aethra.RayTracer.Basic
{
    public struct RayHit
    {
        public Vector3 Position;
        public float Distance;
        public Vector3 Normal;
        public Material Material;
        public Vector2 TextureCoords;
        public int Depth;

        public RayHit(Vector3 position, float distance, Vector3 normal, Material? material = default, Vector2 textureCoords = default)
        {
            Position = position;
            Distance = distance;
            Normal = normal;
            Material = material ?? Material.Error;
            TextureCoords = textureCoords;
            Depth = 0;
        }
    }
}