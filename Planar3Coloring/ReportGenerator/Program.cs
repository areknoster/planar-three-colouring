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
            //string projDir = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            //string folderName = "Data";
            var examples = new List<Example>();
            //for (int i = 1; i <= 9; i++)
            //{
            //    string filename = $"planar_conn.{i}.txt";
            //    string filepath = Path.Combine(projDir, folderName, filename);
            //    var downloadedGraphs = GraphConverter.Convert(filepath);
            //    var downloadedExamples = downloadedGraphs.Select((g) => new Example()
            //    {
            //        Graph = g,
            //        Name = $"downloaded with {g.VertexCount} vertices and {g.EdgeCount} edges",
            //    });
            //    examples.AddRange(downloadedExamples);
            //}
            examples.AddRange(RandomExamplesGenerator.GenerateRandomExamples(57, 5, 0.15));
            var generator = new ReportGenerator();
            generator.RunAlgorithms(examples);
        }
    }
}
