using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planar3Coloring
{
    static class BFS
    {
        public static (List<HashSet<int>>, UndirectedGraph<int, IEdge<int>>, Dictionary<int, (int, int)>) GetBFSTree(UndirectedGraph<int, IEdge<int>> graph, int root)
        {
            UndirectedGraph<int, IEdge<int>> BFSTree = new UndirectedGraph<int, IEdge<int>>();
            List<HashSet<int>> levels = new List<HashSet<int>>();
            Dictionary<int, (int, int)> dict = new Dictionary<int, (int, int)>();

            List<int> visited = new List<int>();
            int level = 0;
            int intNumberOnLevel = 0;
            levels[level] = new HashSet<int>();
            
            List<int> queue = new List<int>();
            int nextLevelMark = -1;
            
            queue.Add(root);
            queue.Add(nextLevelMark);

            while (queue.Count>0)
            {
                int v = queue.First();
                queue.RemoveAt(0);

                //Moving to next level
                if (v == -1)
                {
                    level++;
                    if (queue.Count == 0)
                        break;
                    queue.Add(nextLevelMark);
                    levels[level] = new HashSet<int>();
                    intNumberOnLevel = 0;
                    continue;
                }
               
                levels[level].Add(v);
                visited.Add(v);
                dict.Add(v, (level, intNumberOnLevel));
                BFSTree.AddVertex(v);
                intNumberOnLevel++;

                //Add all v neighbours to queue
                foreach (IEdge<int> e in graph.AdjacentEdges(v))
                    if (!visited.Contains(e.Target))
                    {
                        queue.Add(e.Target);
                        BFSTree.AddVertex(e.Target);
                        BFSTree.AddEdge(new Edge<int>(e.Target, e.Source));
                    }
            }
            return (levels, BFSTree, dict);
        }
    }
}
