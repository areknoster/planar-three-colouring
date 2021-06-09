using System;

namespace ReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var examples = RandomExamplesGenerator.GenerateRandomExamples(50, 10, 0.15);
            var generator = new ReportGenerator(examples);
            generator.RunAlgorithms();
        }
    }
}