using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using Planar3Coloring;
using Planar3Coloring.Test;
using QuikGraph;

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
                examples.Add(new Example($"random with {g.VertexCount} vertices and {g.EdgeCount} edges", g));
            }

            return examples;
        }
    }

    public struct Example
    {
        public Example(string name, UndirectedGraph<int, IEdge<int>> graph)
        {
            Name = name;
            Graph = graph;
        }

        public string Name;
        public UndirectedGraph<int, IEdge<int>> Graph;
    }

    public class ReportGenerator
    {
        private List<IColoringFinder> algorithms;

        public ReportGenerator()
        {
            algorithms = new List<IColoringFinder>()
            {
                new BruteForceColouringFinder(),
                new DnCColoring(),
            };

        }

        private enum Result
        {
            Colorable,
            Uncolorable,
            Error,
        }

        private struct Check
        {
            public TimeSpan elapsed;
            public Result result;
        }

        private void WriteCsv(List<(string, List<Check>)> data)
        {
            using (var w = new StreamWriter("results.csv"))
            {
                foreach (var row in data)
                {
                    var elements = new List<string>();
                    elements.Add(row.Item1);
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
            var data = new List<(string, List<Check>)>(examples.Count);
            foreach (var example in examples)
            {
                data.Add((example.Name, new List<Check>(algorithms.Count)));

                Console.WriteLine(example.Name);
                foreach (var alg in algorithms)
                {
                    var check = new Check();
                    var sw = new Stopwatch();

                    try
                    {
                        sw.Start();
                        var coloring = alg.Find3Colorings(example.Graph);
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
                    }
                    catch
                    {
                        check.result = Result.Error;
                    }


                    check.elapsed = sw.Elapsed;
                    data.Last().Item2.Add(check);
                }
            }

            WriteCsv(data);
        }
    }
}