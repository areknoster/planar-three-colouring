using System;
using System.Collections.Generic;
using System.Linq;
using QuikGraph;

namespace Planar3Coloring.GrahGenerator
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
                g.AddVertex();
            
            return Reshuffler.Reshuffle(g.Graph, rand);
        }
    }
    
}