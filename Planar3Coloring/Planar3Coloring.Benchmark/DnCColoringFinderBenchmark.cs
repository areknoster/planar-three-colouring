using BenchmarkDotNet.Attributes;
using QuikGraph;

namespace Planar3Coloring.Benchmark
{
    [MemoryDiagnoser]
    [RankColumn]
    public class DnCColoringFinderBenchmark
    {
        private static readonly DnCColoringFinder coloringFinder = new DnCColoringFinder();
        private readonly UndirectedGraph<int, IEdge<int>> graph = GraphGenerator.SimpleRandomPlanar(90, 0.15);

        [Benchmark]
        public void DnCColoringBasic()
        {
            coloringFinder.Find3Colorings(graph);
        }
    }
}
