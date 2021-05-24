using QuikGraph;

namespace Planar3Coloring
{
    public interface ITriangulation
    {
        public IGraph<int, IEdge<int>> Triangulate(IGraph<int, IEdge<int>> graph);
    }
}
