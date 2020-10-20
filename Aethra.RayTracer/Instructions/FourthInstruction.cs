using System.Collections.Generic;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Cameras;
using Aethra.RayTracer.Interfaces;
using Aethra.RayTracer.Lighting;
using Aethra.RayTracer.Models;
using Aethra.RayTracer.Primitives;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Instructions
{
    public class FourthInstruction : IInstruction
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
            var camera = new PerspectiveCamera(renderTarget, new Vector3(0f, 0, -10), Vector3.Forward, Vector3.Up);
            var objects = new List<IHittable>();
            var blueMaterial = new PhongMaterial(FloatColor.Blue, 1f, 8, 50, 0.5f);
            var whiteMaterial = new PhongMaterial(FloatColor.White, 1f, 8, 50, 0.5f);
            objects.Add(new Sphere(new Vector3(2.5f, -1, 0), 0.5f, blueMaterial));
            objects.Add(new Sphere(new Vector3(2.5f, -2.5f, 0), 0.75f, whiteMaterial));

            var model = Model.LoadFromFile("_Resources/Models/lowpolytree.obj");

            foreach (var triangle in model)
            {
                objects.Add(triangle);
            }

            Scene = new Scene(objects, camera,
                    new List<Light>
                    {
                        new PointLight {Position = new Vector3(1, 2f, 0), Color = FloatColor.White},
                        new PointLight {Position = new Vector3(-2, -2.5f, 0), Color = FloatColor.Red}
                    }, FloatColor.Black);
        }
    }
}