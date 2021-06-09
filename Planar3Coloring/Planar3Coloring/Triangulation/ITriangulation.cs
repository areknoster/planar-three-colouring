using QuikGraph;

namespace Planar3Coloring.Triangulation
{
    public interface ITriangulation
    {
        public UndirectedGraph<int, IEdge<int>> Triangulate(UndirectedGraph<int, IEdge<int>> graph, int root = 0);
    }
}
