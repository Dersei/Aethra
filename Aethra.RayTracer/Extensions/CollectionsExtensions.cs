using System.Collections.Generic;
using Aethra.RayTracer.Interfaces;
using Aethra.RayTracer.Models;

namespace Aethra.RayTracer.Extensions
{
    public static class CollectionsExtensions
    {
        public static void Add(this List<IHittable> @this, Model model)
        {
            foreach (var triangle in model)
            {
                @this.Add(triangle);
            }
        }
    }
}