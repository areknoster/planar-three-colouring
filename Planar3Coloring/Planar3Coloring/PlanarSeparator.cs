using QuikGraph;
using QuikGraph.Algorithms.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planar3Coloring
{
    static class PlanarSeparator
    {
        public static HashSet<int> FindSeparator(UndirectedGraph<int, IEdge<int>> graph)
        {
            //Phase 1
            int N = graph.VertexCount;
            List<HashSet<int>> BFSLevels;
            UndirectedGraph<int, IEdge<int>> BFSTree;
            Dictionary<int, (int, int)> BFSDict;
            (BFSLevels, BFSTree, BFSDict) = BFS.GetBFSTree(graph, graph.Vertices.First());
            //BFSLevels - HashSet with vertices for each level
            //BFSTree - just BFS tree with edges going down to top
            //BFSDict - Key - int; Value - level and number on level

            //Find mi level
            int nodesNumber = 0;
            int level = 0;
            while (nodesNumber < N / 2)
                nodesNumber += BFSLevels[level++].Count;

            //Assign vertices from mi level to separator
            HashSet<int> S = BFSLevels[level];

            //Check constraints
            if (S.Count < (int)(Math.Sqrt(N) * SeparatorConditions.B))
                return S;

            //Phase 2
            int? m = null, M = null;
            int d = 0;
            while (level - d >= 0 && level + d < BFSLevels.Count() && (!m.HasValue || !M.HasValue))
            {
                if (!m.HasValue && BFSLevels[level - d].Count() < 2 * (Math.Sqrt(N) - d))
                    m = level - d;

                if (!M.HasValue && BFSLevels[level + d].Count() < 2 * (Math.Sqrt(N) - d))
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
            for (int i = 0; i <= m; i++)
                foreach (int v in BFSLevels[i])
                    BFSTree.RemoveVertex(v);
            int root = new int();
            BFSTree.AddVertex(root);
            foreach (int v in BFSLevels[m.Value + 1])
                BFSTree.AddEdge(new Edge<int>(root, v));

            //Reduce levels below M
            for (int i = M.Value; i < BFSLevels.Count(); i++)
                foreach (int v in BFSLevels[i])
                    BFSTree.RemoveVertex(v);

            //Triangulate
            UndirectedGraph<int, IEdge<int>> T = Triangulate(BFSTree);

            HashSet<int> fundamentalCycleVertices = new HashSet<int>();
            int innerVerticesCount = 0;
            int outerVerticesCount = BFSTree.VertexCount;

            IEdge<int> edge = T.Edges.First();

            while (outerVerticesCount > 2 * N / 3 || innerVerticesCount > 2 * N / 3)
            {
                //Find fundamental cycle
                fundamentalCycleVertices = new HashSet<int>();
                int v1 = edge.Source;
                int v2 = edge.Target;

                //Equalize levels
                (int level, int number) v1Pos;
                (int level, int number) v2Pos;
                BFSDict.TryGetValue(edge.Source, out v1Pos);
                BFSDict.TryGetValue(edge.Target, out v2Pos);
                if (v1Pos.level < v2Pos.level)//v1 is always below v2
                {
                    v1 += v2;
                    v2 = v1 - v2;
                    v1 -= v2;
                }
                //Pick nontree edge
                BFSDict.TryGetValue(v1, out v1Pos);
                BFSDict.TryGetValue(v2, out v2Pos);
                (int level, int pos) v3Pos;
                int v3;
                if (outerVerticesCount > innerVerticesCount)//Expand cycle - pick edge incident to edge from lower level
                    foreach(IEdge<int> e in T.AdjacentEdges(v1))
                    { 
                        v3 = e.GetOtherVertex(v1);
                        BFSDict.TryGetValue(v3, out v3Pos);
                        if (v3Pos.level + 1 == v1Pos.level)
                        {
                            fundamentalCycleVertices.Add(v3);
                            v2 = BFSTree.AdjacentEdges(v2).First(e => e.Source == v1).Target;
                            break;
                        }
                    }
                else//Reduce cycle - pick edge incident to edge from upper level
                    foreach(IEdge<int> e in T.AdjacentEdges(v2))
                    {
                        v3 = e.GetOtherVertex(v2);
                        BFSDict.TryGetValue(v3, out v3Pos);
                        if (v3Pos.level - 1 == v2Pos.level)
                        {
                            fundamentalCycleVertices.Add(v3);
                            v2 = BFSTree.AdjacentEdges(v1).First(e => e.Source == v1).Target;
                            break;
                        }
                    }
                //What if no edge to expand or reduce cycle?

                //Find common ancestor 
                while (v1 != v2)
                {
                    BFSDict.TryGetValue(edge.Source, out v1Pos);
                    BFSDict.TryGetValue(edge.Target, out v2Pos);
                    innerVerticesCount += Math.Abs(v1Pos.number - v2Pos.number)-1;

                    //Add vertices to fundamental cycle
                    fundamentalCycleVertices.Add(v1);
                    fundamentalCycleVertices.Add(v2);

                    //Edges in BFSTree are down to top
                    v1 = BFSTree.AdjacentEdges(v1).First(e => e.Source == v1).Target;
                    v2 = BFSTree.AdjacentEdges(v2).First(e => e.Source == v2).Target;
                }
                fundamentalCycleVertices.Add(v1);

                outerVerticesCount = BFSTree.VertexCount - fundamentalCycleVertices.Count - innerVerticesCount;
            }

            //Add vertices from fundamental cycle to separator
            foreach (int v in fundamentalCycleVertices)
                S.Add(v);

            return S;
        }
    }
}