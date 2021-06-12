using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph;

namespace Planar3Coloring.GrahGenerator
{
    class RandomPlanar
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