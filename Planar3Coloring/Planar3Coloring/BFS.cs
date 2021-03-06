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
            UndirectedGraph<int, IEdge<int>> BFSTree = new UndirectedGraph<int, IEdge<int>>(false);
            List<HashSet<int>> levels = new List<HashSet<int>>();
            Dictionary<int, (int, int)> dict = new Dictionary<int, (int, int)>();

            HashSet<int> enqueued = new HashSet<int>();
            int level = 0;
            int intNumberOnLevel = 0;
            levels.Add(new HashSet<int>());
            
            Queue<int> queue = new Queue<int>();
            int nextLevelMark = -1;
            
            queue.Enqueue(root);
            enqueued.Add(root);
            queue.Enqueue(nextLevelMark);

            while (queue.Count>0)
            {
                int v = queue.Dequeue();

                //Moving to next level
                if (v == nextLevelMark)
                {
                    level++;
                    if (queue.Count == 0)
                        break;
                    queue.Enqueue(nextLevelMark);
                    levels.Add(new HashSet<int>());
                    intNumberOnLevel = 0;
                    continue;
                }

                levels[level].Add(v);
                dict.Add(v, (level, intNumberOnLevel));
                intNumberOnLevel++;

                //Add all v neighbours to queue
                foreach (IEdge<int> e in graph.AdjacentEdges(v))
                    if (!enqueued.Contains(e.GetOtherVertex(v)))
                    {
                        queue.Enqueue(e.GetOtherVertex(v));
                        enqueued.Add(e.GetOtherVertex(v));
                        BFSTree.AddVerticesAndEdge(new Edge<int>(v, e.GetOtherVertex(v)));
                    }
            }
            levels.Add(new HashSet<int>());
            return (levels, BFSTree, dict);
        }
    }
}
