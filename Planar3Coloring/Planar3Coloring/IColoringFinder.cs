using System.Collections.Generic;
using QuikGraph;

namespace Planar3Coloring
{
    public interface IColoringFinder
    {
        public IEnumerable<GraphColor?[]> Find3Colorings(IGraph<int, IEdge<int>> graph);
    }
}
