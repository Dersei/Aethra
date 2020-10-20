using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Aethra.RayTracer.Basic;
using Aethra.RayTracer.Basic.Materials;
using Aethra.RayTracer.Interfaces;
using Aethra.RayTracer.Utils;

namespace Aethra.RayTracer.Models
{
    public class Model
    {
        public readonly List<ModelTriangle> Triangles = new List<ModelTriangle>();
        
        public ModelEnumerator GetEnumerator()
        {
            return new ModelEnumerator(Triangles);
        }
        
        public static Model LoadFromFile(string filename, Material? material = default, float scale = 1f,
            Vector3 position = default, Vector3 rotationAxis = default, float angle = 0)
        {
            var obj = new Model();
            try
            {
                using var reader = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
                obj = LoadFromString(reader.ReadToEnd(), material, scale, position, rotationAxis, angle);
            }
            catch (FileNotFoundException)
            {
                Logger.LogConsole($@"File not found: {filename}");
            }
            catch (Exception)
            {
                Logger.LogConsole($@"Error loading file: {filename}");
            }

            return obj;
        }

        private static Model LoadFromString(string text, Material? material, float scale, Vector3 position,
            Vector3 rotationAxis, float angle)
        {
            const NumberStyles style = NumberStyles.Number;
            var culture = CultureInfo.CreateSpecificCulture("en-GB");
            // Separate lines from the file
            var lines = new List<string>(text.Split('\n'));

            // Lists to hold model data
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var textureCoords = new List<Vector2>();
            var faces = new List<(TempVertex, TempVertex, TempVertex)>();

            // Base values
            vertices.Add(new Vector3());
            textureCoords.Add(new Vector2());
            normals.Add(new Vector3());

            // Read file line by line
            foreach (var line in lines)
            {
                if (line.StartsWith("v ")) // Vertex definition
                {
                    // Cut off beginning of line
                    var temp = line.Substring(2);

                    Vector3? vec = null;

                    if (temp.Trim().Count(c => c == ' ') == 2) // Check if there's enough elements for a vertex
                    {
                        var vertexParts = temp.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                        // Attempt to parse each part of the vertice
                        var success = float.TryParse(vertexParts[0], style, culture, out var x);
                        success |= float.TryParse(vertexParts[1], style, culture, out var y);
                        success |= float.TryParse(vertexParts[2], style, culture, out var z);

                        vec = new Vector3(x, y, z);
                        vec = Vector3.Transform(vec.Value, scale, position, rotationAxis, angle);
                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Logger.LogConsole($"Error parsing vertex: {line}");
                        }
                    }
                    else
                    {
                        Logger.LogConsole($"Error parsing vertex: {line}");
                    }

                    vertices.Add(vec ?? new Vector3());
                }
                else if (line.StartsWith("vt ")) // Texture coordinate
                {
                    // Cut off beginning of line
                    var temp = line.Substring(2);

                    Vector2? vec = null;

                    if (temp.Trim().Count(c => c == ' ') > 0) // Check if there's enough elements for a vertex
                    {
                        var texCoordParts = temp.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                        // Attempt to parse each part of the vertex
                        var success = float.TryParse(texCoordParts[0], style, culture, out var x);
                        success |= float.TryParse(texCoordParts[1], style, culture, out var y);

                        vec = new Vector2(x, y);

                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Logger.LogConsole($"Error parsing texture coordinate: {line}");
                        }
                    }
                    else
                    {
                        Logger.LogConsole($"Error parsing texture coordinate: {line}");
                    }

                    textureCoords.Add(vec ?? new Vector2());
                }
                else if (line.StartsWith("vn ")) // Normal vector
                {
                    // Cut off beginning of line
                    var temp = line.Substring(2);

                    Vector3? vec = null;

                    if (temp.Trim().Count(c => c == ' ') == 2) // Check if there's enough elements for a normal
                    {
                        var vertexParts = temp.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                        // Attempt to parse each part of the vertice
                        var success = float.TryParse(vertexParts[0], style, culture, out var x);
                        success |= float.TryParse(vertexParts[1], style, culture, out var y);
                        success |= float.TryParse(vertexParts[2], style, culture, out var z);

                        vec = new Vector3(x, y, z);

                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Logger.LogConsole($"Error parsing normal: {line}");
                        }
                    }
                    else
                    {
                        Logger.LogConsole($"Error parsing normal: {line}");
                    }

                    normals.Add(vec ?? new Vector3());
                }
                else if (line.StartsWith("f ")) // Face definition
                {
                    // Cut off beginning of line
                    var temp = line.Substring(2);

                    if (temp.Trim().Count(c => c == ' ') == 2) // Check if there's enough elements for a face
                    {
                        var faceParts = temp.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                        int t1, t2, t3;
                        int n1, n2, n3;

                        // Attempt to parse each part of the face
                        var success = int.TryParse(faceParts[0].Split('/')[0], out var v1);
                        success |= int.TryParse(faceParts[1].Split('/')[0], out var v2);
                        success |= int.TryParse(faceParts[2].Split('/')[0], out var v3);

                        if (faceParts[0].Count(c => c == '/') >= 2)
                        {
                            success |= int.TryParse(faceParts[0].Split('/')[1], out t1);
                            success |= int.TryParse(faceParts[1].Split('/')[1], out t2);
                            success |= int.TryParse(faceParts[2].Split('/')[1], out t3);
                            success |= int.TryParse(faceParts[0].Split('/')[2], out n1);
                            success |= int.TryParse(faceParts[1].Split('/')[2], out n2);
                            success |= int.TryParse(faceParts[2].Split('/')[2], out n3);
                        }
                        else
                        {
                            if (textureCoords.Count > v1 && textureCoords.Count > v2 && textureCoords.Count > v3)
                            {
                                t1 = v1;
                                t2 = v2;
                                t3 = v3;
                            }
                            else
                            {
                                t1 = 0;
                                t2 = 0;
                                t3 = 0;
                            }


                            if (normals.Count > v1 && normals.Count > v2 && normals.Count > v3)
                            {
                                n1 = v1;
                                n2 = v2;
                                n3 = v3;
                            }
                            else
                            {
                                n1 = 0;
                                n2 = 0;
                                n3 = 0;
                            }
                        }


                        // If any of the parses failed, report the error
                        if (!success)
                        {
                            Logger.LogConsole($"Error parsing face: {line}");
                        }
                        else
                        {
                            var tv1 = new TempVertex(v1, n1, t1);
                            var tv2 = new TempVertex(v2, n2, t2);
                            var tv3 = new TempVertex(v3, n3, t3);
                            faces.Add((tv1, tv2, tv3));
                        }
                    }
                    else
                    {
                        Logger.LogConsole($"Error parsing face: {line}");
                    }
                }
            }

            // Create the Model
            var vol = new Model();

            foreach (var face in faces)
            {
                var v1 = new FaceVertex(vertices[face.Item1.Vertex], normals[face.Item1.Normal],
                    textureCoords[face.Item1.TexCoords]);
                var v2 = new FaceVertex(vertices[face.Item2.Vertex], normals[face.Item2.Normal],
                    textureCoords[face.Item2.TexCoords]);
                var v3 = new FaceVertex(vertices[face.Item3.Vertex], normals[face.Item3.Normal],
                    textureCoords[face.Item3.TexCoords]);

                vol.Triangles.Add(new ModelTriangle(v1, v2, v3, material));
            }

            return vol;
        }
        
        private class TempVertex
        {
            public readonly int Vertex;
            public readonly int Normal;
            public readonly int TexCoords;

            public TempVertex(int vertex = 0, int norm = 0, int tex = 0)
            {
                Vertex = vertex;
                Normal = norm;
                TexCoords = tex;
            }
        }

        public class ModelEnumerator
        {
            private readonly List<ModelTriangle> _triangles;
            public ModelEnumerator(List<ModelTriangle> triangle) => _triangles = triangle;
            private int _currentIndex = -1;
            public bool MoveNext() => ++_currentIndex < _triangles.Count;
            public void Reset() => _currentIndex = -1;
            public ModelTriangle Current => _triangles[_currentIndex];
        }
    }
}