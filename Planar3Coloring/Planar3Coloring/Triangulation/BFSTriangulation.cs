using QuikGraph;
using System;
using System.Collections.Generic;

namespace Planar3Coloring.Triangulation
{
    public class BFSTriangulation : ITriangulation
    {
        /// <summary>
        /// Triangulates a tree, visiting its vertices in BFS manner.
        /// </summary>
        /// <remarks>
        /// Vertices must be from range [0, tree.VertexCount).
        /// </remarks>
        /// <param name="tree">Tree to triangulate. Needs to be connected
        /// or this method triangulates only the component containing <paramref name="root"/>.
        /// Algorithm assumes vertices are numbered as in BFS. If they are not, use another implementation.
        /// </param>
        /// <param name="root"></param>
        /// <returns>Triangulation of <paramref name="tree"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="tree"/> doesn't contain <paramref name="root"/> vertex.</exception>
        public UndirectedGraph<int, IEdge<int>> Triangulate(UndirectedGraph<int, IEdge<int>> tree, int root = 0)
        {
            if (tree.IsVerticesEmpty)
                return tree.Clone();
            if (!tree.ContainsVertex(root))
                throw new ArgumentException("root");

            UndirectedGraph<int, IEdge<int>> G = tree.Clone();

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

            // BFS
            Queue<int> q = new Queue<int>();
            bool[] enqueued = new bool[G.VertexCount];
            q.Enqueue(root);
            enqueued[root] = true;
            while(q.Count > 0)
            {
                int p = q.Dequeue();

                int[] neighbors = new int[embedding[p].Count];
                embedding[p].CopyTo(neighbors);

                int first = neighbors[0];
                if (!enqueued[first])
                {
                    q.Enqueue(first);
                    enqueued[first] = true;
                }
                int prev = first;
                int curr = -1;
                for (int i = 1; i < neighbors.Length; i++)
                {
                    curr = neighbors[i];
                    TryAddEdge(prev, curr, p, ref G, ref embedding);
                    prev = curr;

                    if (!enqueued[curr])
                    {
                        q.Enqueue(curr);
                        enqueued[curr] = true;
                    }
                }
                if (neighbors.Length > 2)
                {
                    TryAddEdge(curr, first, p, ref G, ref embedding);
                }
            }
            return G;
        }

        /// <summary>
        /// Adds an edge, if it doesn't exist, to the graph and its embedding.
        /// </summary>
        /// <param name="v1">A neighbor of <paramref name="parent"/>.</param>
        /// <param name="v2">Consecutive neighbor of <paramref name="parent"/>.</param>
        /// <param name="parent">Common neighbor of <paramref name="v1"/> and <paramref name="v2"/>.</param>
        /// <param name="graph">Graph under triangulation.</param>
        /// <param name="embedding">Combinatorial embedding of <paramref name="graph"/></param>
        private void TryAddEdge(int v1, int v2, int parent, ref UndirectedGraph<int, IEdge<int>> graph, ref List<int>[] embedding)
        {
            if (graph.ContainsEdge(v1, v2))
                return;

            graph.AddEdge(new Edge<int>(v1, v2));
            int idx1 = embedding[v1].FindIndex(v => v == parent);
            int idx2 = embedding[v2].FindIndex(v => v == parent);
            embedding[v1].Insert(idx1, v2);
            embedding[v2].Insert(idx2 + 1, v1);
        }
    }
}
