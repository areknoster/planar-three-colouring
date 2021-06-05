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
        public static (List<HashSet<Vertex>>, AdjacencyGraph<Vertex, IEdge<Vertex>>, Dictionary<Vertex, (int, int)>) GetBFSTree(AdjacencyGraph<Vertex, IEdge<Vertex>> graph, Vertex root)
        {
            AdjacencyGraph<Vertex, IEdge<Vertex>> BFSTree = new AdjacencyGraph<Vertex, IEdge<Vertex>>();
            List<HashSet<Vertex>> levels = new List<HashSet<Vertex>>();
            Dictionary<Vertex, (int, int)> dict = new Dictionary<Vertex, (int, int)>();

            List<Vertex> visited = new List<Vertex>();
            int level = 0;
            int vertexNumberOnLevel = 0;
            levels[level] = new HashSet<Vertex>();
            
            List<Vertex> queue = new List<Vertex>();
            Vertex nextLevelMark = null;
            
            queue.Add(root);
            queue.Add(nextLevelMark);

            while (queue.Count>0)
            {
                Vertex v = queue.First();
                queue.RemoveAt(0);

                //Moving to next level
                if (v == null)
                {
                    level++;
                    if (queue.Count == 0)
                        break;
                    queue.Add(nextLevelMark);
                    levels[level] = new HashSet<Vertex>();
                    vertexNumberOnLevel = 0;
                    continue;
                }
               
                levels[level].Add(v);
                visited.Add(v);
                dict.Add(v, (level, vertexNumberOnLevel));
                BFSTree.AddVertex(v);
                vertexNumberOnLevel++;

                //Add all v neighbours to queue
                foreach (IEdge<Vertex> e in graph.OutEdges(v))
                    if (!visited.Contains(e.Target))
                    {
                        queue.Add(e.Target);
                        BFSTree.AddVertex(e.Target);
                        BFSTree.AddEdge(new Edge<Vertex>(e.Target, e.Source));
                    }
            }
            return (levels, BFSTree, dict);
        }
    }
}
