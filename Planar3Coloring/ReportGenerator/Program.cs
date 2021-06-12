using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Planar3Coloring.ColoringFinder;
using Planar3Coloring.ColoringFinder.DnCColoringFinder;

namespace ReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var variousExamples = new List<Example>();
            
            for (double density = 0.05; density <= 0.5; density += 0.05)
            {
                variousExamples.AddRange( RandomExamplesGenerator.GenerateRandomExamples(20, 10, density));
            }
            
            var allAlgorithms = new List<IColoringFinder>()
            {
                new BruteForceColouringFinder(),
                new DnCColoringBasic(),
                new DnCColoringParallel()
            };
            var generator = new ReportGenerator(TimeSpan.FromSeconds(10), allAlgorithms);
            generator.RunAlgorithms(variousExamples);
            generator.WriteCsv("all_algorithms.csv");
            
            // test just DnC
            var longExamples = RandomExamplesGenerator.GenerateRandomExamples(50, 20, 0.1);
            var justDnCGenerator = new ReportGenerator(
                TimeSpan.FromMinutes(1),
                new List<IColoringFinder>() {new DnCColoringBasic()}
            );
            justDnCGenerator.RunAlgorithms(longExamples);
            justDnCGenerator.WriteCsv("just_dnc.csv");

        }
    }
}