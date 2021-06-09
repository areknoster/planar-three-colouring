using System.Collections.Generic;
using QuikGraph;
using System.Linq;
using System;

namespace Planar3Coloring
{
    public class BruteForceColouringFinder : IColoringFinder
    {
        public string Name => "BruteForceColoring";

        public GraphColor[] Find3Colorings(UndirectedGraph<int, IEdge<int>> graph)
        {
            var finder = new GraphColoringFinder(graph);
            return finder.Find();
        }

        private class GraphColoringFinder
        {
            private GraphColor?[] coloring;
            private UndirectedGraph<int, IEdge<int>> graph;

            public GraphColoringFinder(UndirectedGraph<int, IEdge<int>> graph)
            {
                this.graph = graph;
                this.coloring = new GraphColor?[graph.VertexCount];
            }

            public GraphColor[] Find()
            {
                if (!TryColors(0))
                {
                    return null;
                }
                return coloring.Select((GraphColor? c) => c.Value).ToArray();
            }

            private bool TryColors(int vertex)
            {
                if (vertex == graph.VertexCount) {
                    return true;
                }
                var available = new SortedSet<GraphColor> { GraphColor.White, GraphColor.Gray, GraphColor.Black };
                foreach (int v in graph.AdjacentVertices(vertex))
                {
                    if (coloring[v] is not null) {
                        available.Remove(coloring[v].Value);
                    }
                    
                }

                var next = vertex + 1;
                foreach (GraphColor c in available)
                {
                    coloring[vertex] = c;
                    if (TryColors(next))
                    {
                        return true;
                    }
                }
                coloring[vertex] = null;
                return false;
            }
        }
    }
}
