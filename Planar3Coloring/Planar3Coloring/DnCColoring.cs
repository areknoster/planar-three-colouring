using QuikGraph;
using QuikGraph.Algorithms.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planar3Coloring
{
    public class DnCColoring : IColoringFinder
    {
        private UndirectedGraph<int, IEdge<int>> _graph;
        private HashSet<GraphColor>[] _availableColors;
        private GraphColor?[] _coloring;
        public GraphColor[] Find3Colorings(UndirectedGraph<int, IEdge<int>> graph)
        {
            //Tests
            //foreach (IEdge<int> edge in graph.Edges)
            //    Console.WriteLine(edge);


            //Initialize structures
            _graph = graph.Clone();
            _availableColors = new HashSet<GraphColor>[_graph.VertexCount];
            for (int i = 0; i < _graph.VertexCount; i++)
                _availableColors[i] = new HashSet<GraphColor>() { GraphColor.Black, GraphColor.Gray, GraphColor.White };
            _coloring = new GraphColor?[_graph.VertexCount];

            //Get all components of G
            List<UndirectedGraph<int, IEdge<int>>> G_components = FindComponents(_graph);
            foreach (UndirectedGraph<int, IEdge<int>> component in G_components)
            {
                //Find separator S for component
                HashSet<int> S = PlanarSeparator.FindSeparator(component);
                foreach (int v in S)
                    component.RemoveVertex(v);
                //Split component into smaller components
                List<UndirectedGraph<int, IEdge<int>>> components = FindComponents(component);

                //One of components cannot be colored => whole G cannot be colored
                if (!BruteForceColoring(components, S))
                    return null;
            }
            return _coloring.Select(c => c.Value).ToArray();
        }

        public string Name => "DnCColoring";

        private bool BruteForceColoring(List<UndirectedGraph<int, IEdge<int>>> components, HashSet<int> s)
        {
            if (s.Count == 0)
                throw new Exception("Empty separator");
            int v = s.First();
            s.Remove(v);

            foreach (GraphColor color in _availableColors[v])
            {
                //Color v with color
                _coloring[v] = color;
                bool moveToNextColor = false;
                List<int> verticesWithTakenColor = new List<int>();

                foreach(int n in _graph.AdjacentVertices(v))
                {
                    if(!_coloring[n].HasValue && _availableColors[n].Contains(color))
                    {
                        //Adjacent vertices cannot be colored with color
                        _availableColors[n].Remove(color);
                        verticesWithTakenColor.Add(n);

                        //Adjacent not colored vertex has no available color - invalid coloring 
                        if(_availableColors[n].Count==0)
                        {
                            //Reverse changes
                            foreach (int vertex in verticesWithTakenColor)
                                _availableColors[vertex].Add(color);
                            //Move to next v coloring
                            moveToNextColor = true;
                            break;
                        }
                    }
                }
                if (moveToNextColor)
                    continue;

                if (s.Count > 0)//Continue coloring separator
                {
                    if (BruteForceColoring(components, s))
                        return true;
                }
                else //SeparatorColored
                {
                    foreach(UndirectedGraph<int, IEdge<int>> component in components)
                    {
                        if (component.VertexCount < 5)
                        {
                            //Can't find separator for component - color using bruteforce
                            if (!BruteForceColoring(new List<UndirectedGraph<int, IEdge<int>>>(),
                                                    new HashSet<int>(component.Vertices)))
                            {
                                //Component cannot be colored => move to next v coloring
                                moveToNextColor = true;
                                break;
                            }
                        }
                        else
                        {
                            HashSet<int> sPrim = PlanarSeparator.FindSeparator(component);
                            foreach (int vPrim in sPrim)
                                component.RemoveVertex(vPrim);
                            List<UndirectedGraph<int, IEdge<int>>> componentsPrim = FindComponents(component);
                            if(!BruteForceColoring(componentsPrim, sPrim))
                            {                              
                                //One of components could not be colored => move to next v coloring
                                moveToNextColor = true;
                                break;
                            }    
                        }
                    }
                    if (moveToNextColor)
                    {
                        //Reverse changes - could not color
                        foreach (int vertex in verticesWithTakenColor)
                            _availableColors[vertex].Add(color);
                        continue;
                    }
                    return true;
                }
                //Reverse changes
                foreach (int vertex in verticesWithTakenColor)
                    _availableColors[vertex].Add(color);
            }
            return false;
        }
        private List<UndirectedGraph<int, IEdge<int>>> FindComponents(UndirectedGraph<int, IEdge<int>> g)
        {
            List<UndirectedGraph<int, IEdge<int>>> components = new List<UndirectedGraph<int, IEdge<int>>>();
            HashSet<int> vertices = g.Vertices.ToHashSet();
            while(vertices.Count>0)
            {
                UndirectedGraph<int, IEdge<int>> component = new UndirectedGraph<int, IEdge<int>>(false);
                Queue<int> queue = new Queue<int>();
                int v = vertices.First();
                vertices.Remove(v);
                queue.Enqueue(v);
                component.AddVertex(v);
                while(queue.Count>0)
                {
                    v = queue.Dequeue();
                    foreach(int n in g.AdjacentVertices(v))
                    {
                        if(vertices.Contains(n))
                        {
                            component.AddVertex(n);
                            queue.Enqueue(n);
                            vertices.Remove(n);
                        }
                        component.AddEdge(new Edge<int>(v, n));
                    }
                }
                components.Add(component);
            }
            return components;
        }
    }
}
