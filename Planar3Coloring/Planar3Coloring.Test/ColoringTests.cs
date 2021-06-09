using System;
using System.Collections.Generic;
using Xunit;
using QuikGraph;
using Planar3Coloring;
using System.Diagnostics;

namespace Planar3Coloring.Test
{
    public class ColoringTests
    {
        private List<UndirectedGraph<int, IEdge<int>>> examples;
        private List<IColoringFinder> algorithms;
        public ColoringTests()
        {
            algorithms = new List<IColoringFinder>()
            {
                new BruteForceColouringFinder(),
                new DnCColoring(),
            };
            LoadExamples();
        }
        
        private void LoadExamples()
        {
            examples = new List<UndirectedGraph<int, IEdge<int>>>();
            for (int i = 3; i < 10; i++)
            {
                examples.Add(GraphGenerator.SimpleRandomPlanar(i, 0.2));
            }
        }


        [Fact]
        public void BruteForceVersusOptimized()
        {
            foreach (var graph in examples)
            {
                Console.WriteLine($"Graph with {graph.Vertices} verices and {graph.Edges} edges");
                foreach (var alg in algorithms)
                {
                    Console.WriteLine($"Starting algorithm {alg.Name}");
                    var sw = new Stopwatch();
                    sw.Start();
                    var coloring = alg.Find3Colorings(graph);
                    sw.Stop();
                    if (coloring != null)
                    {
                        Assert.True(ColoringChecker.CheckColoring(graph, coloring));
                    }

                    Console.WriteLine("Elapsed={0}",sw.Elapsed);
                }
            }
        }
        
    }
}