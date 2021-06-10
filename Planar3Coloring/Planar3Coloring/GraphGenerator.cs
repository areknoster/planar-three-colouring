using System;
using System.Collections;
using QuikGraph;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace Planar3Coloring
{

    public static class GraphGenerator
    {
        public static UndirectedGraph<int, IEdge<int>> Clique(int vertices)
        {
            var g = new UndirectedGraph<int, IEdge<int>>(false);
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
            while (g.Count < vertices)
            {
                g.AddVertex();
            }

            return g.Graph;
        }

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
                foreach (var vertex in Vertices.Where(vertex => _random.NextDouble() < density))
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
                    var faceVertices = new List<int>(){newVertex};
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
                            faceVertices.Add(Vertices[(first + index) % Vertices.Count ]);
                            index++;
                        }
                        
                    }

                    faces.Add(new Face(_random, faceVertices));
                }


                return faces;
            }
        }
        
        private class RandomPlanar
        {
            private List<Face> _faces;
            public UndirectedGraph<int, IEdge<int>> Graph { get; }
            private readonly double _density;
            private Random random;
            public int Count => Graph.VertexCount;
            public RandomPlanar(double density, Random random)
            {
                _density = Math.Clamp(density, 0, 1);
                this.random = random;
                Graph = new UndirectedGraph<int, IEdge<int>>();
                Graph.AddVerticesAndEdge(new Edge<int>(0, 1));
                _faces = new List<Face>()
                {
                    new Face(random, new List<int>{0,1})
                };

            }
            
            public void AddVertex()
            {
                //choose face randomly
                var faceIndex = random.Next(_faces.Count);
                var face = _faces[faceIndex];
                
                var connections = face.ChooseConnections(_density);
                if (connections.Count == 0) //skipping if no connection was found, because the graph must stay connected.
                    return;

                int newVertex = Graph.VertexCount;
                // update graph with new vertex
                Graph.AddVertex(newVertex);
                Graph.AddEdgeRange(connections.Select((vSource) => new Edge<int>(vSource, newVertex)));
                
                //update state of the rest of the instance
                var newFaces = face.DivideByConnections(newVertex, connections);
                _faces.RemoveAt(faceIndex);
                _faces.AddRange(newFaces);
            }

        }
        
    }
    
}