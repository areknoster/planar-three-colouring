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

            for (double density = 0.05; density < 0.15; density += 0.02)
            {
                examples.AddRange( RandomExamplesGenerator.GenerateRandomExamples(10, 5, density));
            }
           
            var generator = new ReportGenerator(TimeSpan.FromSeconds(10));
            generator.RunAlgorithms(examples);
        }
    }
}