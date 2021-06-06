using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planar3Coloring
{
    static class ColoringCheck
    {
        public static bool IsColoringValid(UndirectedGraph<int, IEdge<int>> g, GraphColor[] coloring)
        {
            foreach (IEdge<int> e in g.Edges)
                if (coloring[e.Source] == coloring[e.Target])
                    return false;
            return true;
        }
    }
}
