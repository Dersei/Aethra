namespace Aethra.RayTracer.Basic.Textures
{
    public class CheckerTexture : ITexture
    {
        public FloatColor FirstColor = FloatColor.White;
        public FloatColor SecondColor = FloatColor.Black;

        public FloatColor GetColor(Vector2 uv, Vector2 scale, Vector2 offset) => GetColor(uv);

        public FloatColor GetColor(Vector2 uv)
        {
            var sum = (int) (uv.X / 3 - 100 + float.Epsilon) // lame
                      + (int) (uv.Y / 3 - 100 + float.Epsilon);
            //+ (int)(position.Z / 3 - 100 + float.Epsilon);

            if (sum % 2 != 0)
            {
                return SecondColor;
            }

            return FirstColor;
        }
        
        public static CheckerTexture Create() => new CheckerTexture();
        public static CheckerTexture Create(FloatColor firstColor, FloatColor secondColor) => new CheckerTexture
        {
            FirstColor = firstColor,
            SecondColor = secondColor
        };
    }
}