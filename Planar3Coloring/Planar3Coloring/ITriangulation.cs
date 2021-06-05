using QuikGraph;

namespace Planar3Coloring
{
    public interface ITriangulation
    {
        public UndirectedGraph<int, IEdge<int>> Triangulate(UndirectedGraph<int, IEdge<int>> graph);
    }
}
