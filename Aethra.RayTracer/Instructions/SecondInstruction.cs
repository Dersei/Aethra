using System.Collections.Generic;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Cameras;
using Aethra.RayTracer.Interfaces;
using Aethra.RayTracer.Lighting;
using Aethra.RayTracer.Primitives;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Instructions
{
    public class SecondInstruction : IInstruction
    {
        public Scene? Scene { private set; get; }
        public uint[,]? Result => Scene?.Camera.RenderTarget.Pixels;

        public IEnumerable<bool> Render()
        {
            return Scene!.Render();
        }

        public void CreateScene(int width, int height, FloatColor color, bool useAntialiasing)
        {
            var renderTarget = new Framebuffer(width, height);
            renderTarget.Clear(color);
            var camera = new PerspectiveCamera(renderTarget, new Vector3(0f, 5, 0), Vector3.Down, Vector3.Forward);
            var objects = new List<IHittable>();
            var blueMaterial = new PhongMaterial(FloatColor.Blue, 1f, 8, 50, 0.5f);
            var redMaterial = new PhongMaterial(FloatColor.Red, 1f, 8, 50, 0.5f);
            objects.Add(new Sphere(new Vector3(0, 2, 0), 0.4f, blueMaterial));
            objects.Add(new Sphere(new Vector3(0.5f, 0f, 0), 0.3f, redMaterial));

            var colorTo255 = 0;
            const float oneThird = 1 / 3f;
            const float twoThird = 2 / 3f;
            for (float j = -1; j < 1; j += oneThird)
            {
                objects.Add(new Quad(new Vector3(-1, 0, -j), new Vector3(-1, 0, -j - oneThird),
                    new Vector3(-twoThird, 0, -j - oneThird), new Vector3(-twoThird, 0, -j),
                    FloatColor.FromRGBA(colorTo255, 0, 0)));

                objects.Add(new Quad(new Vector3(-twoThird, 0, -j), new Vector3(-twoThird, 0, -j - oneThird),
                    new Vector3(-oneThird, 0, -j - oneThird), new Vector3(-oneThird, 0, -j),
                    FloatColor.FromRGBA(0, colorTo255, 0)));

                objects.Add(new Quad(new Vector3(-oneThird, 0, -j), new Vector3(-oneThird, 0, -j - oneThird),
                    new Vector3(0, 0, -j - oneThird), new Vector3(0, 0, -j),
                    FloatColor.FromRGBA(0, 0, colorTo255)));

                objects.Add(new Quad(new Vector3(0, 0, -j), new Vector3(0, 0, -j - oneThird),
                    new Vector3(oneThird, 0, -j - oneThird), new Vector3(oneThird, 0, -j),
                    FloatColor.FromRGBA(255, 0, colorTo255)));

                objects.Add(new Quad(new Vector3(oneThird, 0, -j), new Vector3(oneThird, 0, -j - oneThird),
                    new Vector3(twoThird, 0, -j - oneThird), new Vector3(twoThird, 0, -j),
                    FloatColor.FromRGBA(0, 255, colorTo255)));

                objects.Add(new Quad(new Vector3(twoThird, 0, -j), new Vector3(twoThird, 0, -j - oneThird),
                    new Vector3(1f, 0, -j - oneThird), new Vector3(1f, 0, -j),
                    FloatColor.FromRGBA(255, 255, colorTo255)));

                colorTo255 += (int) 42.5F;
            }

            Scene = new Scene(objects, camera,new List<Light>(), FloatColor.Black);
        }
    }
}