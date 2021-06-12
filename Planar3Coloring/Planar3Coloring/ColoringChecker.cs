using System;
using QuikGraph;
namespace Planar3Coloring
{
    public static class ColoringChecker
    {
        public static bool CheckColoring(UndirectedGraph<int, IEdge<int>> graph, GraphColor[] coloring)
        {
            foreach (var e in graph.Edges)
            {
                if (coloring[e.Source] == coloring[e.Target])
                {
                    return false;
                }
            }
            return true;
        }
    }

}

