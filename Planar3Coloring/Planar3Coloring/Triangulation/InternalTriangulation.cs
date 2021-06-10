using QuikGraph;
using System;
using System.Collections.Generic;

namespace Planar3Coloring.Triangulation
{
    public class InternalTriangulation : ITriangulation
    {
        /// <summary>
        /// Triangulates a tree, visiting its vertices in order as they are stored internally.
        /// </summary>
        /// <remarks>
        /// Vertices must be from range [0, tree.VertexCount).
        /// </remarks>
        /// <param name="tree">Tree to triangulate. Needs to be connected
        /// or this method triangulates only the component containing <paramref name="root"/>.
        /// Algorithm assumes vertices are numbered as in BFS. If they are not, use another implementation.
        /// </param>
        /// <param name="root">This vertex has no special meaning. It must be present in the graph though.</param>
        /// <returns>Triangulation of <paramref name="tree"/>.</returns>
        public UndirectedGraph<int, IEdge<int>> Triangulate(UndirectedGraph<int, IEdge<int>> tree, int root = 0)
        {
            if (tree.IsVerticesEmpty)
                return tree.Clone();
            if (!tree.ContainsVertex(root))
                throw new ArgumentException("root");

            UndirectedGraph<int, IEdge<int>> G = tree.Clone();
            UndirectedGraph<int, IEdge<int>> res = new UndirectedGraph<int, IEdge<int>>();

            // create counter-clockwise combinatorial embedding
            // assumes vertices are numbered as in BFS
            // so it's proper to add them to list in order they are stored internally
            List<int>[] embedding = new List<int>[G.VertexCount];
            for (int i = 0; i < G.VertexCount; i++)
            {
                embedding[i] = new List<int>();
                foreach (int v in G.AdjacentVertices(i))
                    embedding[i].Add(v);
            }

            foreach (int v in tree.Vertices)
            {
                int[] neighbors = new int[embedding[v].Count];
                embedding[v].CopyTo(neighbors);

                int prev = neighbors[0];
                for (int i = 1; i < neighbors.Length; i++)
                {
                    int curr = neighbors[i];
                    TryAddEdge(prev, curr, v, ref G, ref res, ref embedding);
                    prev = curr;
                }
                if (neighbors.Length > 2)
                    TryAddEdge(prev, neighbors[0], v, ref G, ref res, ref embedding);
            }

            return res;
        }

        /// <summary>
        /// Adds an edge, if it doesn't exist, to the graph and its embedding.
        /// </summary>
        /// <param name="v1">A neighbor of <paramref name="parent"/>.</param>
        /// <param name="v2">Consecutive neighbor of <paramref name="parent"/>.</param>
        /// <param name="parent">Common neighbor of <paramref name="v1"/> and <paramref name="v2"/>.</param>
        /// <param name="graph">Graph under triangulation.</param>
        /// <param name="embedding">Combinatorial embedding of <paramref name="graph"/></param>
        private void TryAddEdge(int v1, int v2, int parent, ref UndirectedGraph<int, IEdge<int>> graph, ref UndirectedGraph<int, IEdge<int>> graph2, ref List<int>[] embedding)
        {
            if (graph.ContainsEdge(v1, v2))
                return;

            graph.AddEdge(new Edge<int>(v1, v2));
            graph2.AddVerticesAndEdge(new Edge<int>(v1, v2));
            int idx1 = embedding[v1].FindIndex(v => v == parent);
            int idx2 = embedding[v2].FindIndex(v => v == parent);
            embedding[v1].Insert(idx1, v2);
            embedding[v2].Insert(idx2 + 1, v1);
        }
    }
}
