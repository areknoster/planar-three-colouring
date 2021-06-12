using System.Collections.Generic;
using QuikGraph;

namespace Planar3Coloring.ColoringFinder
{
    public interface IColoringFinder
    {
        public GraphColor[] Find3Colorings(UndirectedGraph<int, IEdge<int>> graph);
        public string Name { get; }
    }
}
