using System.Collections.Generic;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Cameras;
using Aethra.RayTracer.Interfaces;
using Aethra.RayTracer.Lighting;
using Aethra.RayTracer.Models;
using Aethra.RayTracer.Primitives;
using Aethra.RayTracer.Rendering;

namespace Aethra.RayTracer.Instructions
{
    public class ThirdInstruction : IInstruction
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

            objects.Add(new Sphere(new Vector3(0, -2, 0), 0.75f));

            var model = Model.LoadFromFile("_Resources/Models/lowpolytree.obj");
            
            foreach (var triangle in model)
            {
                objects.Add(triangle);
            }

            Scene = new Scene(objects, camera,new List<Light>(), FloatColor.Black);
            Scene.Camera.SpecialColoring = (_, hit) => FloatColor.FromNormal(hit.Normal);
        }
    }
}