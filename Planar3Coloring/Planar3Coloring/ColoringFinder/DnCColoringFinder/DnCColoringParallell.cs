//using QuikGraph;
//using QuikGraph.Algorithms.Search;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Planar3Coloring.ColoringFinder.DnCColoringFinder
//{
//    public class DnCColoringParallell : IColoringFinder
//    {
//        private UndirectedGraph<int, IEdge<int>> _graph;
//        private HashSet<GraphColor>[] _availableColors;
//        private GraphColor?[] _coloring;
//        public GraphColor[] Find3Colorings(UndirectedGraph<int, IEdge<int>> graph)
//        {
//            //Initialize structures
//            _graph = graph.Clone();
//            _availableColors = new HashSet<GraphColor>[_graph.VertexCount];
//            for (int i = 0; i < _graph.VertexCount; i++)
//                _availableColors[i] = new HashSet<GraphColor>() { GraphColor.Black, GraphColor.Gray, GraphColor.White };
//            _coloring = new GraphColor?[_graph.VertexCount];

//            //Get all components of G
//            List<UndirectedGraph<int, IEdge<int>>> G_components = FindComponents(_graph);
//            foreach (UndirectedGraph<int, IEdge<int>> component in G_components)
//            {
//                //Find separator S for component
//                HashSet<int> S = PlanarSeparator.FindSeparator(component);
//                ////Split component into smaller components
//                foreach (int v in S)
//                    component.RemoveVertex(v);
//                List<UndirectedGraph<int, IEdge<int>>> components = FindComponents(component);

//                //One of components cannot be colored => whole G cannot be colored
//                //Slow version
//                if (!DnCColoring(components, S))
//                    return null;
//                //Fast version
//                //if (!DnCColoring(new List<UndirectedGraph<int, IEdge<int>>>(), _graph.Vertices.ToHashSet()))
//                //    return null;
//            }
//            return _coloring.Select(c => c.Value).ToArray();
//        }

//        public string Name => "DnCColoring";

//        private bool DnCColoring(List<UndirectedGraph<int, IEdge<int>>> components, HashSet<int> s)
//        {
//            //Separator colored
//            if (s.Count == 0)
//            {
//                bool canBeColored = true;
//                Parallel.ForEach(components, (UndirectedGraph<int, IEdge<int>> component, ParallelLoopState state) =>
//                {
//                    if (component.VertexCount < 15)
//                    {
//                        //Can't find separator for component - color using bruteforce
//                        if (!DnCColoring(new List<UndirectedGraph<int, IEdge<int>>>(),
//                                                new HashSet<int>(component.Vertices)))
//                        {
//                            canBeColored = false;
//                            state.Break();
//                        }
//                    }
//                    else
//                    {
//                        HashSet<int> sPrim = PlanarSeparator.FindSeparator(component);
//                        UndirectedGraph<int, IEdge<int>> componentCopy = component.Clone();
//                        foreach (int vPrim in sPrim)
//                            componentCopy.RemoveVertex(vPrim);
//                        List<UndirectedGraph<int, IEdge<int>>> componentsPrim = FindComponents(componentCopy);
//                        if (!DnCColoring(componentsPrim, sPrim))
//                        {
//                            canBeColored = false;
//                            state.Break();
//                        }
//                    }
//                });
//                return canBeColored;
//            }

//            //Separator still not colored
//            int v = s.First();
//            s.Remove(v);
//            foreach (GraphColor color in (GraphColor[])Enum.GetValues(typeof(GraphColor)))
//            {
//                if (!_availableColors[v].Contains(color))
//                    continue;

//                //Color v with color
//                _coloring[v] = color;
//                bool moveToNextColor = false;
//                List<int> verticesWithTakenColor = new List<int>();

//                foreach (int n in _graph.AdjacentVertices(v))
//                    if (_availableColors[n].Contains(color))
//                    {
//                        //Adjacent not colored vertex has no available color - invalid coloring 
//                        if (_availableColors[n].Count == 1)
//                        {
//                            //Reverse changes
//                            foreach (int vertex in verticesWithTakenColor)
//                                _availableColors[vertex].Add(color);
//                            //Move to next v coloring
//                            moveToNextColor = true;
//                            break;
//                        }

//                        //Adjacent vertices cannot be colored with color
//                        _availableColors[n].Remove(color);
//                        verticesWithTakenColor.Add(n);
//                    }

//                if (moveToNextColor)
//                    continue;

//                if (DnCColoring(components, s))
//                    return true;

//                //Reverse changes
//                foreach (int vertex in verticesWithTakenColor)
//                    _availableColors[vertex].Add(color);
//            }
//            s.Add(v);
//            return false;
//        }
//        private List<UndirectedGraph<int, IEdge<int>>> FindComponents(UndirectedGraph<int, IEdge<int>> g)
//        {
//            List<UndirectedGraph<int, IEdge<int>>> components = new List<UndirectedGraph<int, IEdge<int>>>();
//            HashSet<int> vertices = g.Vertices.ToHashSet();
//            while (vertices.Count > 0)
//            {
//                UndirectedGraph<int, IEdge<int>> component = new UndirectedGraph<int, IEdge<int>>(false);
//                Queue<int> queue = new Queue<int>();
//                int v = vertices.First();
//                vertices.Remove(v);
//                queue.Enqueue(v);
//                component.AddVertex(v);
//                while (queue.Count > 0)
//                {
//                    v = queue.Dequeue();
//                    foreach (int n in g.AdjacentVertices(v))
//                    {
//                        if (vertices.Contains(n))
//                        {
//                            component.AddVertex(n);
//                            queue.Enqueue(n);
//                            vertices.Remove(n);
//                        }
//                        component.AddEdge(new Edge<int>(v, n));
//                    }
//                }
//                components.Add(component);
//            }
//            return components;
//        }
//    }
//}
