using System.Collections.Generic;
using QuikGraph;

namespace Planar3Coloring
{
    public interface IColoringFinder
    {
        public IEnumerable<GraphColor[]> Find3Colorings(UndirectedGraph<int, IEdge<int>> graph);
    }
}
