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
                variousExamples.AddRange( RandomExamplesGenerator.GenerateRandomExamples(10, 10, density));
            }
            
            var allAlgorithms = new List<IColoringFinder>()
            {
                new BruteForceColouringFinder(),
                new DnCColoringBasic(),
                new DnCColoringImproved(),
                new DnCColoringParallel()
            };
            var generator = new ReportGenerator(TimeSpan.FromMinutes(1), allAlgorithms);
            generator.RunAlgorithms(variousExamples, TimeSpan.FromMinutes(60));
            generator.WriteCsv("all_algorithms.csv");
            
            // test just DnC
            var longExamples = RandomExamplesGenerator.GenerateRandomExamples(50, 20, 0.1);
            var justDnCGenerator = new ReportGenerator(
                TimeSpan.FromMinutes(20),
                new List<IColoringFinder>() {new DnCColoringImproved()}
            );
            justDnCGenerator.RunAlgorithms(longExamples, TimeSpan.FromMinutes(60));
            justDnCGenerator.WriteCsv("just_dnc.csv");

        }
    }
}