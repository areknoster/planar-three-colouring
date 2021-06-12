using System;
using System.Collections.Generic;
using System.Linq;

namespace Planar3Coloring.GrahGenerator
{
    public class Face
    {
        public Face(Random random, List<int> vertices)
        {
            _random = random;
            Vertices = vertices;
        }

        private Random _random;
        public List<int> Vertices { get; }

        public HashSet<int> ChooseConnections(double density)
        {
            var connections = new HashSet<int>();
            foreach (var vertex in Vertices.Where((_) => _random.NextDouble() < density))
            {
                connections.Add(vertex);
            }

            return connections;
        }

        public List<Face> DivideByConnections(int newVertex, HashSet<int> connections)
        {
            var faces = new List<Face>(connections.Count);
            int first = Vertices.FindIndex(v => connections.Contains(v));
            int index = 0;
            // the ranges for first n-1 faces are set by connections, the last consists of the rest
            for (int j = 0; j < connections.Count; j++)
            {
                var faceVertices = new List<int>() {newVertex};
                if (j < connections.Count - 1) // normal casae
                {
                    faceVertices.Add(Vertices[(first + index) % Vertices.Count]);
                    int v = -1;
                    while (!connections.Contains(v))
                    {
                        index++;
                        v = Vertices[(first + index) % Vertices.Count];
                        faceVertices.Add(v);
                    }
                }
                else // last part
                {
                    while (index <= Vertices.Count)
                    {
                        faceVertices.Add(Vertices[(first + index) % Vertices.Count]);
                        index++;
                    }
                }

                faces.Add(new Face(_random, faceVertices));
            }


            return faces;
        }
    }
}