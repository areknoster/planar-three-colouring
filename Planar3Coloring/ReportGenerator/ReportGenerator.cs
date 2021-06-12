using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic;
using Planar3Coloring;
using Planar3Coloring.ColoringFinder;
using Planar3Coloring.ColoringFinder.DnCColoringFinder;
using Planar3Coloring.GrahGenerator;
using QuikGraph;
using System.Threading.Tasks;


namespace ReportGenerator
{
    public static class RandomExamplesGenerator
    {
        public static List<Example> GenerateRandomExamples(int numberOfRandom, int step, double density)
        {
            var examples = new List<Example>(numberOfRandom);
            for (int i = 0; i <= numberOfRandom; i++)
            {
                var g = GraphGenerator.SimpleRandomPlanar(i * step, density);
                examples.Add(new Example($"random connected", g, density));
            }

            return examples;
        }
    }

    public struct Example
    {
        public Example(string name, UndirectedGraph<int, IEdge<int>> graph, double density)
        {
            Name = name;
            Graph = graph;
            Density = density;
        }

        public string Name;
        public int VerticesCount => Graph.VertexCount;
        public int EdgesCount => Graph.EdgeCount;
        public double Density;
        public UndirectedGraph<int, IEdge<int>> Graph;
    }

    public class ReportGenerator
    {
        private List<(Example, List<Check>)> _data;
        private List<IColoringFinder> algorithms;
        private TimeSpan _timeout;

        public ReportGenerator(TimeSpan timeout, List<IColoringFinder> algorithms)
        {
            _timeout = timeout;
            this.algorithms = algorithms;
            _data = new List<(Example, List<Check>)>();
        }

        private enum Result
        {
            Colorable,
            Uncolorable,
            Error,
            Timeout,
        }

        private struct Check
        {
            public TimeSpan elapsed;
            public Result result;
        }

        public void WriteCsv(string path)
        {
            using (var w = new StreamWriter(path))
            {
                //write header
                var header = new List<string>() {"Graph", "Vertices", "Edges", "Density"};
                foreach (var alg in algorithms)
                {
                    var columns = new List<string>()
                    {
                        $"{alg.Name}_elapsed_ms",
                        $"{alg.Name}_result",
                    };
                    header.AddRange(columns);
                }
                w.WriteLine(Strings.Join(header.ToArray(), ","));
                foreach (var row in _data)
                {
                    var elements = new List<string>();
                    elements.AddRange(new string[]
                    {
                        row.Item1.Name, 
                        row.Item1.VerticesCount.ToString(), 
                        row.Item1.EdgesCount.ToString(), 
                        row.Item1.Density.ToString(),
                    });
                    foreach (var check in row.Item2)
                    {
                        elements.Add(check.elapsed.ToString());
                        elements.Add(check.result.ToString());
                    }

                    w.WriteLine(Strings.Join(elements.ToArray(), ","));
                }
            }
        }

        public void RunAlgorithms(List<Example> examples)
        {
            foreach (var example in examples)
                {
                    _data.Add((example, new List<Check>(algorithms.Count)));

                    Console.WriteLine($"{example.Name} vertices={example.VerticesCount} edges={example.EdgesCount} density={example.Density} ");
                    foreach (var alg in algorithms)
                    {
                        var check = new Check();
                        var sw = new Stopwatch();
                        GraphColor[] coloring = new GraphColor[] { };
                        
                        sw.Start();
                        var task = Task.Run(() => coloring = alg.Find3Colorings(example.Graph));
                        if (!task.Wait(_timeout))
                        {
                            check.result = Result.Timeout;
                            check.elapsed = _timeout;
                            _data.Last().Item2.Add(check);
                            Console.WriteLine($"{alg.Name} v={example.VerticesCount} e={example.EdgesCount} : Timeout");
                            continue;
                        }
                        sw.Stop();
                        
                        if (coloring != null)
                        {
                            if (!ColoringChecker.CheckColoring(example.Graph, coloring))
                            {
                                throw new Exception("Wrong coloring output!");
                            }

                            check.result = Result.Colorable;
                        }
                        else
                        {
                            check.result = Result.Uncolorable;
                        }
                        check.elapsed = sw.Elapsed;
                        Console.WriteLine($"{alg.Name}: elapsed={check.elapsed.TotalMilliseconds} result={check.result.ToString()}");
                        _data.Last().Item2.Add(check);
                    }
                }
        }
    }
}