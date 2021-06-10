using QuikGraph;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReportGenerator
{
    public static class GraphConverter
    {
        public static List<UndirectedGraph<int, IEdge<int>>> Convert(string filepath)
        {
            StreamReader file = new StreamReader(filepath);
            List<UndirectedGraph<int, IEdge<int>>> graphs = new List<UndirectedGraph<int, IEdge<int>>>();

            UndirectedGraph<int, IEdge<int>> currGraph = null;// = new UndirectedGraph<int, IEdge<int>>();
            string line = file.ReadLine();
            while (line != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    line = file.ReadLine();
                    continue;
                }

                if (line.StartsWith("Graph"))
                {
                    if (!(currGraph is null))
                        graphs.Add(currGraph);
                    currGraph = new UndirectedGraph<int, IEdge<int>>(false);
                }
                else if (line.StartsWith("  "))
                {
                    string[] verts = line.Split(' ', ':', ';').Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    int[] vertices = verts.Select(s => int.Parse(s)).ToArray();
                    currGraph.AddVertex(vertices[0]);
                    for (int i = 1; i < vertices.Length; i++)
                    {
                        currGraph.AddVerticesAndEdge(new Edge<int>(vertices[0], vertices[i]));
                    }
                }

                line = file.ReadLine();
            }
            if (!(currGraph is null))
                graphs.Add(currGraph);
            return graphs;
        }
    }
}
