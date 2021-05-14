using QuikGraph;

namespace Planar3Coloring
{
    interface ITriangulation<TVertex>
    {
        public IGraph<TVertex, IEdge<TVertex>> Triangulate(IGraph<TVertex, IEdge<TVertex>> graph);
    }
}
