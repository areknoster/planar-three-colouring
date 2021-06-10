using System;

namespace ReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            //int noVertices = 3;
            //string projDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            //string folderName = "Data";
            //string filename = $"planar_conn.{noVertices}.txt";
            //string filepath = Path.Combine(projDir, folderName, filename);

            //var graphs = GraphConverter.Convert(filepath);
            var examples = RandomExamplesGenerator.GenerateRandomExamples(50, 10, 0.15);
            var generator = new ReportGenerator(examples);
            generator.RunAlgorithms();
        }
    }
}