using System;
using QuikGraph;
namespace Planar3Coloring.Test
{
    public static class ColoringChecker
    {
        public static bool CheckColoring(UndirectedGraph<int, IEdge<int>> graph, GraphColor[] coloring)
        {
            for (int i = 0; i < graph.VertexCount; i++)
            {
                var neighbors = graph.AdjacentVertices(i);
                var c = coloring[i];
                foreach (int n in neighbors)
                {
                    if (coloring[n] == c)
                    {
                        throw new Exception(
                            $"coloring is incorrect: adjacent verices {i} and {n} are of the same color");
                    }
                }
            }
            return true;
        }
    }

}

