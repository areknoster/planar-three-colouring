using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var examples = new List<Example>();
            examples.AddRange( RandomExamplesGenerator.GenerateRandomExamples(30, 10, 0.15));
            var generator = new ReportGenerator();
            generator.RunAlgorithms(examples);
        }
    }
}