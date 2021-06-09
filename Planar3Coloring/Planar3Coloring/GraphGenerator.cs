using System;
using QuikGraph;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Planar3Coloring
{

    public static class GraphGenerator
    {
        public static UndirectedGraph<int, IEdge<int>> Clique(int vertices)
        {
            var g = new UndirectedGraph<int, IEdge<int>>();
            for (int i = 0; i < vertices; i++)
            {
                g.AddVertex(i);
                for (int j = 0; j < i; j++)
                {
                    g.AddEdge(new Edge<int>(j, i));
                }
            }

            return g;
        }

        public static UndirectedGraph<int, IEdge<int>> SimpleRandomPlanar(int vertices, double denisty, int seed = 0)
        {
            var rand = new Random(seed);
            var g = new RandomPlanar(denisty, rand);
            vertices = vertices < 3 ? 3 : vertices;

            while (g.Count < vertices)
            {
                g.AddVertex();
            }

            return g.Graph;
        }
        
        private class RandomPlanar
        {
            private List<List<int>> _faces;
            public UndirectedGraph<int, IEdge<int>> Graph { get; }
            private readonly double _density;
            private Random random;
            public int Count => Graph.VertexCount;
            public RandomPlanar(double density, Random random)
            {
                this._density = Math.Clamp(density, 0, 1);
                this.random = random;
                this.Graph = GraphGenerator.Clique(3);
                _faces = new List<List<int>>
                {
                    new List<int>(this.Graph.Vertices), // internal
                    new List<int>(this.Graph.Vertices), // external
                };
            }

            public void AddVertex()
            {
                int newVertex = Graph.VertexCount;
                Graph.AddVertex(newVertex);
                //choose face randomly
                var faceIndex = random.Next(_faces.Count);
                var face = _faces[faceIndex];
                var connections = new List<int> (ChooseConnectionsFromFace(face));
                var ranges = new List<(int, int)>();
                for (int i = 0; i < connections.Count; i++)
                {
                    var r = (connections[i], connections[(i + 1) % connections.Count]);
                    ranges.Add(r);
                    Graph.AddEdge(new Edge<int>(r.Item1, newVertex));
                }
                var newFaces = ranges.Select((r) =>
                {
                    var newFace = new List<int>();
                    for (int i = r.Item1; i != r.Item2; i = (i + 1) % newFace.Count)
                        newFace.Add(face[i]);
                    newFace.Add(face[r.Item2]);
                    return newFace;
                });
                _faces.RemoveAt(faceIndex);
                _faces.AddRange(newFaces);
            }
            
            // chooses at least two random connections from new vertices based on density
            private SortedSet<int> ChooseConnectionsFromFace(List<int> face)
            {
                var connections = new SortedSet<int>();
                for (int i = 0; i < face.Count; i++)
                {
                    if (random.NextDouble() <= _density)
                    {
                        connections.Add(i);
                    }
                }
                while (connections.Count < 2)
                {
                    connections.Add(random.Next(face.Count));
                }
                return connections;
            }
        }
        
    }
    
}