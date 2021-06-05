using QuikGraph;
using QuikGraph.Algorithms.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planar3Coloring
{
    class PlanarSeparator
    {
        private AdjacencyGraph<Vertex, IEdge<Vertex>> Graph { get; set; }
        private int N { get; set; }
        private int SeparatorSize { get; set; }
        public PlanarSeparator(AdjacencyGraph<Vertex, IEdge<Vertex>> graph)
        {
            Graph = graph.Clone();
            N = graph.Vertices.Count();
            SeparatorSize = (int)(Math.Sqrt(N) * SeparatorConditions.B);
        }
        public HashSet<Vertex> FindSeparator()
        {
            //Phase 1
            List<HashSet<Vertex>> BFSLevels;
            AdjacencyGraph<Vertex, IEdge<Vertex>> BFSTree;
            Dictionary<Vertex, (int, int)> BFSDict;
            (BFSLevels, BFSTree, BFSDict) = BFS.GetBFSTree(Graph, Graph.Vertices.First());
            //BFSLevels - HashSet with vertices for each level
            //BFSTree - just BFS tree with edges going down to top
            //BFSDict - Key - vertex; Value - level and number on level

            //Find mi level
            int nodesNumber = 0;
            int level = 0;
            while (nodesNumber < N / 2)
                nodesNumber += BFSLevels[level++].Count;

            //Assign vertices from mi level to separator
            HashSet<Vertex> S = BFSLevels[level];

            //Check constraints
            if (S.Count < SeparatorSize)
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
            foreach (Vertex v in BFSLevels[M.Value])
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
                foreach (Vertex v in BFSLevels[i])
                    BFSTree.RemoveVertex(v);
            Vertex root = new Vertex();
            BFSTree.AddVertex(root);
            foreach (Vertex v in BFSLevels[m.Value + 1])
                BFSTree.AddEdge(new Edge<Vertex>(root, v));

            //Reduce levels below M
            for (int i = M.Value; i < BFSLevels.Count(); i++)
                foreach (Vertex v in BFSLevels[i])
                    BFSTree.RemoveVertex(v);
           
            //Triangulate
            AdjacencyGraph<Vertex, IEdge<Vertex>> T = Triangulate(BFSTree);

            HashSet<Vertex> fundamentalCycleVertices = new HashSet<Vertex>();
            int innerVerticesCount = 0;
            int outerVerticesCount = BFSTree.VertexCount;

            List<IEdge<Vertex>> nonTreeEdges = T.Edges.ToList();            
            foreach (IEdge<Vertex> e in BFSTree.Edges)
                nonTreeEdges.Remove(e);

            IEdge<Vertex> edge = nonTreeEdges[0];

            while (outerVerticesCount > 2 * N / 3 || innerVerticesCount > 2 * N / 3)
            {
                //Pick nontree edge
                if (outerVerticesCount>innerVerticesCount)//Expand cycle - pick edge incident to edge from lower level
                {
                    
                }
                else//Reduce cycle - pick edge incident to edge from upper level
                {

                }

                //Find fundamental cycle
                Vertex source = edge.Source;
                Vertex target = edge.Target;

                //Equalize levels
                (int level, int number) sourcePos;
                (int level, int number) targetPos;
                BFSDict.TryGetValue(edge.Source, out sourcePos);
                BFSDict.TryGetValue(edge.Target, out targetPos);
                if (sourcePos.level > targetPos.level) //source is below target
                {
                    fundamentalCycleVertices.Add(source);
                    source = BFSTree.OutEdges(source).First().Target;
                }
                else
                {
                    fundamentalCycleVertices.Add(target);
                    target = BFSTree.OutEdges(target).First().Target;
                }

                //Find common ancestor 
                while (source != target)
                {
                    BFSDict.TryGetValue(edge.Source, out sourcePos);
                    BFSDict.TryGetValue(edge.Target, out targetPos);
                    innerVerticesCount += Math.Abs(sourcePos.level - targetPos.level);

                    //Add vertices to fundamental cycle
                    fundamentalCycleVertices.Add(source);
                    fundamentalCycleVertices.Add(target);

                    //Edges in BFSTree are down to top
                    source = BFSTree.OutEdges(source).First().Target;
                    target = BFSTree.OutEdges(target).First().Target;
                }
                fundamentalCycleVertices.Add(source);

                outerVerticesCount = BFSTree.VertexCount - fundamentalCycleVertices.Count - innerVerticesCount;
            }

            //Add vertices from fundamental cycle to separator
            foreach (Vertex v in fundamentalCycleVertices)
                S.Add(v);

            return S;
        }
    }
}