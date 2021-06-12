using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using Planar3Coloring.Triangulation;

namespace Planar3Coloring
{
    static class PlanarSeparator
    {
        public static HashSet<int> FindSeparator(UndirectedGraph<int, IEdge<int>> graph)
        {
            int N = graph.VertexCount;
            List<HashSet<int>> BFSLevels;
            UndirectedGraph<int, IEdge<int>> BFSTree;
            Dictionary<int, (int level, int number)> BFSDict;
            (BFSLevels, BFSTree, BFSDict) = BFS.GetBFSTree(graph, graph.Vertices.Last());
            //BFSLevels - HashSet with vertices for each level
            //BFSTree - just BFS tree with edges going down to top
            //BFSDict - Key - int; Value - level and number on level

            //Find mi level
            int nodesNumber = 0;
            int level = -1;
            while (nodesNumber < N / 2)
                nodesNumber += BFSLevels[++level].Count;

            //Assign vertices from mi level to separator
            HashSet<int> S = BFSLevels[level];

            //Check constraints
            if (S.Count <= (Math.Sqrt(N) * SeparatorConditions.B))
                return S;

            //Phase 2
            int? m = null, M = null;
            int d = 0;
            while ((level - d >= 0 || level + d < BFSLevels.Count()) && (!m.HasValue || !M.HasValue))
            {
                if (!m.HasValue && (level - d >= 0) && BFSLevels[level - d].Count() <= 2 * (Math.Sqrt(N) - d))
                    m = level - d;

                if (!M.HasValue && (level + d < BFSLevels.Count()) && BFSLevels[level + d].Count() <= 2 * (Math.Sqrt(N) - d))
                    M = level + d;
                d++;
            }

            S = BFSLevels[m.Value];
            foreach (int v in BFSLevels[M.Value])
                S.Add(v);

            //Check constraints
            int topCount = 0;
            for (int i = 0; i < m.Value; i++)
                topCount += BFSLevels[i].Count();
            int midCount = 0;
            for (int i = m.Value + 1; i < M.Value; i++)
                midCount += BFSLevels[i].Count();
            int botCount = 0;
            for (int i = M.Value + 1; i < BFSLevels.Count; i++)
                botCount += BFSLevels[i].Count();

            int BCount = Math.Max(Math.Max(topCount, midCount), botCount);
            int ACount = topCount + midCount + botCount - BCount;

            if (ACount / (double)BCount < SeparatorConditions.Balance)
                return S;

            //Phase 3
            //Reduce 0 to m levels
            int root = BFSLevels[0].First();
            for (int i = 0; i <= m; i++)
                foreach (int v in BFSLevels[i])
                    BFSTree.RemoveVertex(v);
            BFSDict[root] = (m.Value, 0);

            BFSTree.AddVertex(root);
            foreach (int v in BFSLevels[m.Value + 1])
                BFSTree.AddEdge(new Edge<int>(root, v));

            //Reduce levels below M
            for (int i = M.Value; i < BFSLevels.Count(); i++)
                foreach (int v in BFSLevels[i])
                    BFSTree.RemoveVertex(v);

            //Triangulate
            var triangulization = new InternalTriangulation();
            (UndirectedGraph<int, IEdge<int>> T, Dictionary<(int source, int target), int> innerVertices) = triangulization.Triangulate(BFSTree, root);

            HashSet<int> fundamentalCycleVertices = new HashSet<int>();
            int innerVerticesCount = 0;
            int outerVerticesCount = BFSTree.VertexCount;

            IEnumerator<IEdge<int>> triangEdges = T.Edges.GetEnumerator();

            while (triangEdges.MoveNext() && outerVerticesCount > 2 * N / 3 || innerVerticesCount > 2 * N / 3)
            {
                IEdge<int> edge = triangEdges.Current;

                fundamentalCycleVertices = new HashSet<int>();
                int v1 = edge.Source;
                int v2 = edge.Target;

                fundamentalCycleVertices.Add(v1);
                fundamentalCycleVertices.Add(v2);

                while (BFSDict[v1].level > BFSDict[v2].level)
                {
                    v1 = GetParentVertex(v1, BFSTree, BFSDict);
                    fundamentalCycleVertices.Add(v1);
                }

                while (BFSDict[v2].level > BFSDict[v1].level)
                {
                    v2 = GetParentVertex(v2, BFSTree, BFSDict);
                    fundamentalCycleVertices.Add(v2);
                }

                while (v1 != v2)
                {
                    fundamentalCycleVertices.Add(v1);
                    fundamentalCycleVertices.Add(v2);
                    v1 = GetParentVertex(v1, BFSTree, BFSDict);
                    v2 = GetParentVertex(v2, BFSTree, BFSDict);
                }
                fundamentalCycleVertices.Add(v1);

                innerVerticesCount = innerVertices.ContainsKey((edge.Source, edge.Target)) ?
                                        innerVertices[(edge.Source, edge.Target)] :
                                        innerVertices[(edge.Target, edge.Source)];
                outerVerticesCount = BFSTree.VertexCount - fundamentalCycleVertices.Count - innerVerticesCount;
            }

            //Add vertices from fundamental cycle to separator
            foreach (int v in fundamentalCycleVertices)
                S.Add(v);

            return S;
        }

        private static int GetParentVertex(int v, UndirectedGraph<int, IEdge<int>> BFSTree, Dictionary<int, (int level, int number)> BFSDict)
        {
            return BFSTree.AdjacentEdges(v).First(e => BFSDict[e.GetOtherVertex(v)].level == BFSDict[v].level - 1).GetOtherVertex(v);
        }
    }
}
