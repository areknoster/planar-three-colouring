using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                var g =  GraphGenerator.SimpleRandomPlanar(i * step, density);
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
        public void RunAlgorithms()
        {
            foreach (var example in examples)
            {
                Console.WriteLine(example.Name);
                foreach (var alg in algorithms)
                {
                    Console.WriteLine($"Starting algorithm {alg.Name}");
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
                        Console.WriteLine("correct coloring output");
                    }
                    else
                    {
                        Console.WriteLine("Graph is not colorable");
                    }

                    Console.WriteLine("Elapsed={0}",sw.Elapsed);
                }
            }
        }
        
    }
}