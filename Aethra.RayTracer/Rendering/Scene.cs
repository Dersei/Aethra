using System.Collections.Generic;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Cameras;
using Aethra.RayTracer.Interfaces;
using Aethra.RayTracer.Lighting;

namespace Aethra.RayTracer.Rendering
{
    public class Scene
    {
        public readonly List<IHittable> Objects;
        public readonly List<Light> Lights;
        public readonly FloatColor Background;
        public readonly Camera Camera;

        public Scene(List<IHittable> objects, Camera camera, List<Light> lights, FloatColor background = default)
        {
            Objects = objects;
            Lights = lights;
            Camera = camera;
            camera.Scene = this;
            Background = background;
        }

        public IEnumerable<bool> Render()
        {
            return Camera.Render();
        }

        public IEnumerable<bool> Render(int startHeight, int endHeight, int startWidth, int endWidth)
        {
            return Camera.Render(startHeight, endHeight, startWidth, endWidth);
        }

        public bool TestRay(Ray ray, out RayHit result)
        {
            result = new RayHit();
            var isHit = false;
            var minimalDistance = float.MaxValue;

            foreach (var obj in Objects)
            {
                if (obj.Hit(ray, out var temp) && temp.Distance < minimalDistance)
                {
                    minimalDistance = temp.Distance;
                    isHit = true;
                    result = temp;
                }
            }

            return isHit;
        }

        public bool IsAnythingBetween(Vector3 origin, Vector3 target)
        {
            var between = target - origin;
            var distance = between.Length;

            var ray = new Ray(origin, between.Normalize(), 1E-4f, float.MaxValue);
            foreach (var obj in Objects)
            {
                if (obj.Hit(ray, out var temp) &&
                    temp.Distance < distance /* && !(temp.Material is TransparentMaterial)*/)
                {
                    return true;
                }
            }

            return false;
        }
    }
}