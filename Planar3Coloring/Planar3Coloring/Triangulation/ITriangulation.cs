using QuikGraph;
using System.Collections.Generic;

namespace Planar3Coloring.Triangulation
{
    public interface ITriangulation
    {
        public (UndirectedGraph<int, IEdge<int>> nonTreeEdges, Dictionary<(int source, int target), int> innerVertices)
            Triangulate(UndirectedGraph<int, IEdge<int>> graph, int root = 0);
    }
}
