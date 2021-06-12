using BenchmarkDotNet.Running;

namespace Planar3Coloring.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            // run in Release configuration!
            // after-run artifacts are stored in bin\Release\net5.0\BenchmarkDotNet.Artifacts\results
            BenchmarkRunner.Run<DnCColoringFinderBenchmark>();
        }
    }
}
