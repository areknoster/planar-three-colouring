using System;

namespace ReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var generator = new ReportGenerator(1000, 0.15);
            generator.BruteForceVersusOptimized();
        }
    }
}