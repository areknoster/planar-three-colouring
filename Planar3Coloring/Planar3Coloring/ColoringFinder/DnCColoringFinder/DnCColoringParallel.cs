using QuikGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Planar3Coloring.ColoringFinder.DnCColoringFinder
{
    public class DnCColoringParallel : IColoringFinder
    {
        private UndirectedGraph<int, IEdge<int>> _graph;
        private HashSet<GraphColor>[] _availableColors;
        private GraphColor?[] _coloring;
        public GraphColor[] Find3Colorings(UndirectedGraph<int, IEdge<int>> graph)
        {
            //Initialize structures
            _graph = graph.Clone();
            _availableColors = new HashSet<GraphColor>[_graph.VertexCount];
            for (int i = 0; i < _graph.VertexCount; i++)
                _availableColors[i] = new HashSet<GraphColor>() { GraphColor.Black, GraphColor.Gray, GraphColor.White };
            _coloring = new GraphColor?[_graph.VertexCount];

            //Get all components of G
            (List<UndirectedGraph<int, IEdge<int>>> list, Dictionary<int, int> dict) G_components = FindComponents(_graph);
            foreach (UndirectedGraph<int, IEdge<int>> component in G_components.list)
            {
                //Find separator S for component
                HashSet<int> S = PlanarSeparator.FindSeparator(component);
                ////Split component into smaller components
                foreach (int v in S)
                    component.RemoveVertex(v);
                (List<UndirectedGraph<int, IEdge<int>>>, Dictionary<int, int>) components = FindComponents(component);

                //One of components cannot be colored => whole G cannot be colored
                if (!DnCColoring(components, S).isColorable)
                    return null;
            }
            return _coloring.Select(c => c.Value).ToArray();
        }

        public string Name => "DnCColoringParallel";

        private (bool isColorable, int? notColorableComponentIndex) DnCColoring((List<UndirectedGraph<int, IEdge<int>>> list, Dictionary<int, int> dict) components, HashSet<int> s)
        {
            //Separator colored
            if (s.Count == 0)
            {
                bool canBeColored = true;
                int? notColorableComponentIndex = null;
                Parallel.ForEach(components.list, (UndirectedGraph<int, IEdge<int>> component, ParallelLoopState state) =>
                {
                    notColorableComponentIndex = components.list.IndexOf(component);
                    if (component.VertexCount < 15)
                    {
                        //Can't find separator for component - color using bruteforce
                        if (!DnCColoring((new List<UndirectedGraph<int, IEdge<int>>>(), new Dictionary<int, int>()),
                                                new HashSet<int>(component.Vertices)).isColorable)
                        {
                            canBeColored = false;
                            state.Break();
                        }
                    }
                    else
                    {
                        HashSet<int> sPrim = PlanarSeparator.FindSeparator(component);
                        UndirectedGraph<int, IEdge<int>> componentCopy = component.Clone();
                        foreach (int vPrim in sPrim)
                            componentCopy.RemoveVertex(vPrim);
                        (List<UndirectedGraph<int, IEdge<int>>>, Dictionary<int, int>) componentsPrim = FindComponents(componentCopy);
                        if (!DnCColoring(componentsPrim, sPrim).isColorable)
                        {
                            canBeColored = false;
                            state.Break();
                        }
                    }
                });
                return (canBeColored, notColorableComponentIndex);
            }

            //Separator still not colored
            (bool isColorable, int? componentIndex) = (true, null);
            int v = s.First();
            s.Remove(v);
            foreach (GraphColor color in (GraphColor[])Enum.GetValues(typeof(GraphColor)))
            {
                if (!_availableColors[v].Contains(color))
                    continue;

                //Color v with color
                _coloring[v] = color;
                bool moveToNextColor = false;
                List<int> verticesWithTakenColor = new List<int>();

                foreach (int n in _graph.AdjacentVertices(v))
                    if (_availableColors[n].Contains(color))
                    {
                        //Adjacent not colored vertex has no available color - invalid coloring 
                        if (_availableColors[n].Count == 1)
                        {
                            //Reverse changes
                            foreach (int vertex in verticesWithTakenColor)
                                _availableColors[vertex].Add(color);
                            //Move to next v coloring
                            moveToNextColor = true;
                            break;
                        }

                        //Adjacent vertices cannot be colored with color
                        _availableColors[n].Remove(color);
                        verticesWithTakenColor.Add(n);
                    }

                if (moveToNextColor)
                    continue;

                if (!componentIndex.HasValue)
                    (isColorable, componentIndex) = DnCColoring(components, s);

                if (isColorable)
                    return (true, null);

                //Reverse changes
                foreach (int vertex in verticesWithTakenColor)
                {
                    _availableColors[vertex].Add(color);
                    if (componentIndex.HasValue && components.dict.ContainsKey(vertex) && components.dict[vertex] == componentIndex)
                        componentIndex = null;
                }
            }
            s.Add(v);
            return (false, componentIndex);
        }
        private (List<UndirectedGraph<int, IEdge<int>>>, Dictionary<int, int>) FindComponents(UndirectedGraph<int, IEdge<int>> g)
        {
            List<UndirectedGraph<int, IEdge<int>>> components = new List<UndirectedGraph<int, IEdge<int>>>();
            Dictionary<int, int> componentBelonging = new Dictionary<int, int>();
            HashSet<int> vertices = g.Vertices.ToHashSet();
            int componentNumber = 0;
            while (vertices.Count > 0)
            {
                UndirectedGraph<int, IEdge<int>> component = new UndirectedGraph<int, IEdge<int>>(false);
                Queue<int> queue = new Queue<int>();
                int v = vertices.First();
                vertices.Remove(v);
                queue.Enqueue(v);
                component.AddVertex(v);
                while (queue.Count > 0)
                {
                    v = queue.Dequeue();
                    foreach (int n in g.AdjacentVertices(v))
                    {
                        if (vertices.Contains(n))
                        {
                            component.AddVertex(n);
                            componentBelonging.Add(n, componentNumber);
                            queue.Enqueue(n);
                            vertices.Remove(n);
                        }
                        component.AddEdge(new Edge<int>(v, n));
                    }
                }
                components.Add(component);
                componentNumber++;
            }
            return (components, componentBelonging);
        }
    }
}
