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
        public (UndirectedGraph<int, IEdge<int>> nonTreeEdges, Dictionary<(int source, int target), int> innerVertices)
            Triangulate(UndirectedGraph<int, IEdge<int>> tree, int root = 0)
        {
            UndirectedGraph<int, IEdge<int>> res = new UndirectedGraph<int, IEdge<int>>();
            Dictionary<(int source, int target), int> innerVerticesCount = new Dictionary<(int source, int target), int>();
            if (tree.IsVerticesEmpty)
                return (res, innerVerticesCount);
            if (!tree.ContainsVertex(root))
                throw new ArgumentException("root");

            UndirectedGraph<int, IEdge<int>> G = tree.Clone();
            // create counter-clockwise combinatorial embedding
            // assumes vertices are numbered as in BFS
            // so it's proper to add them to list in order they are stored internally
            Dictionary<int, List<int>> embedding = new Dictionary<int, List<int>>();
            for (int i = 0; i < G.VertexCount; i++)
            {
                embedding.Add(i, new List<int>());
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
                    TryAddEdge(prev, curr, p, ref G, ref res, ref embedding, ref innerVerticesCount);
                    prev = curr;

                    if (!enqueued[curr])
                    {
                        q.Enqueue(curr);
                        enqueued[curr] = true;
                    }
                }
                if (neighbors.Length > 2)
                {
                    TryAddEdge(curr, first, p, ref G, ref res, ref embedding, ref innerVerticesCount);
                }
            }
            return (res, innerVerticesCount);
        }

        /// <summary>
        /// Adds edge v1-v2, if it doesn't exist, to the graph and its embedding.
        /// </summary>
        /// <param name="v1">A neighbor of <paramref name="parent"/>.</param>
        /// <param name="v2">Consecutive neighbor of <paramref name="parent"/>.</param>
        /// <param name="parent">Common neighbor of <paramref name="v1"/> and <paramref name="v2"/>.</param>
        /// <param name="graph">Graph under triangulation.</param>
        /// <param name="embedding">Combinatorial embedding of <paramref name="graph"/></param>
        private void TryAddEdge(int v1, int v2, int parent,
            ref UndirectedGraph<int, IEdge<int>> graph,
            ref UndirectedGraph<int, IEdge<int>> addedEdgesGraph,
            ref Dictionary<int, List<int>> embedding,
            ref Dictionary<(int source, int target), int> innerVerticesCount)
        {
            if (graph.ContainsEdge(v1, v2))
                return;

            bool isE1Added = addedEdgesGraph.ContainsEdge(parent, v1);
            bool isE2Added = addedEdgesGraph.ContainsEdge(parent, v2);
            int vertsCount = 0;
            if (isE1Added)
            {
                int vertsCount1;
                if (!innerVerticesCount.TryGetValue((parent, v1), out vertsCount1))
                    innerVerticesCount.TryGetValue((v1, parent), out vertsCount1);
                vertsCount += vertsCount1;
            }
            if (isE2Added)
            {
                int vertsCount2;
                if (!innerVerticesCount.TryGetValue((parent, v1), out vertsCount2))
                    innerVerticesCount.TryGetValue((v1, parent), out vertsCount2);
                vertsCount += vertsCount2;
            }
            if (isE1Added && isE2Added)
                vertsCount += 1;
            innerVerticesCount.Add((v1, v2), vertsCount);

            graph.AddEdge(new Edge<int>(v1, v2));
            addedEdgesGraph.AddVerticesAndEdge(new Edge<int>(v1, v2));
            int idx1 = embedding[v1].FindIndex(v => v == parent);
            int idx2 = embedding[v2].FindIndex(v => v == parent);
            embedding[v1].Insert(idx1, v2);
            embedding[v2].Insert(idx2 + 1, v1);
        }
    }
}
