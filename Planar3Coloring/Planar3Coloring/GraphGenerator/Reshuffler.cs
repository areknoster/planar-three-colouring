using System;
using System.Linq;
using QuikGraph;

namespace Planar3Coloring.GrahGenerator
{
    // Reshuffler permutatex the vertices of a graph without chaning its structure
    public static class Reshuffler
    {
        public static UndirectedGraph<int, IEdge<int>> Reshuffle(UndirectedGraph<int, IEdge<int>> g, Random rnd)
        {
            int[] mapping = g.Vertices.OrderBy(_ => rnd.Next()).ToArray();
            var reshuffled = new UndirectedGraph<int, IEdge<int>>(false);
            reshuffled.AddVerticesAndEdgeRange(
                g.Edges.Select(e => new Edge<int>(mapping[e.Source], mapping[e.Target]))
            );
            return reshuffled;
        }
        
    }
}