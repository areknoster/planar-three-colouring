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
        private List<Example> examples;
        private List<IColoringFinder> algorithms;

        public ReportGenerator(List<Example> examples)
        {
            algorithms = new List<IColoringFinder>()
            {
                new BruteForceColouringFinder(),
                new DnCColoring(),
            };
            this.examples = examples;
        }

        private enum Result
        {
            Colorable,
            Uncolorable
        }

        private struct Check
        {
            public TimeSpan elapsed;
            public Result result;
        }

        private void WriteCSV(List<List<Check>> data)
        {
            using (var w = new StreamWriter("results.csv"))
            {
                foreach (var checks in data)
                {
                    var elements = new List<string>();
                    foreach (var check in checks)
                    {
                        elements.Add(check.elapsed.ToString());
                        elements.Add(check.result.ToString());
                    }
                    w.WriteLine(Strings.Join(elements.ToArray(), ","));
                }
            }
        }
        public void RunAlgorithms()
        {
            var data = new List<List<Check>>(examples.Count);
            foreach (var example in examples)
            {
                data.Add(new List<Check>(algorithms.Count));
                
                Console.WriteLine(example.Name);
                foreach (var alg in algorithms)
                {
                    var check = new Check();
                    var sw = new Stopwatch();
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

                    check.elapsed = sw.Elapsed;
                    data.Last().Add(check);
                }
            }
            WriteCSV(data);
        }
    }
}