using System.Collections.Generic;
using QuikGraph;

namespace Planar3Coloring
{
    public interface IColoringFinder
    {
        public GraphColor[] Find3Colorings(UndirectedGraph<int, IEdge<int>> graph);
        public string Name { get; }
    }
}
