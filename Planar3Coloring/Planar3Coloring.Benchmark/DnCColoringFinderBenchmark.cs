using BenchmarkDotNet.Attributes;
using Planar3Coloring.ColoringFinder;
using Planar3Coloring.ColoringFinder.DnCColoringFinder;
using Planar3Coloring.GrahGenerator;
using QuikGraph;

namespace Planar3Coloring.Benchmark
{
    [MemoryDiagnoser]
    [RankColumn]
    public class DnCColoringFinderBenchmark
    {
        private readonly UndirectedGraph<int, IEdge<int>> graph = GraphGenerator.SimpleRandomPlanar(70, 0.15);

        [Benchmark]
        public void DnCColoringBasic()
        {
            IColoringFinder coloringFinder = new DnCColoringBasic();
            coloringFinder.Find3Colorings(graph);
        }

        [Benchmark]
        public void DnCColoringParallel()
        {
            IColoringFinder coloringFinder = new DnCColoringParallel();
            coloringFinder.Find3Colorings(graph);
        }
    }
}
