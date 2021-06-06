using Xunit;
using QuikGraph;
using Planar3Coloring;
using System.Collections.Generic;
using System.Collections;

namespace Planar3Coloring.Test
{
    //public class TestDataGenerator : IEnumerable<object[]>
    //{
    //    private readonly List<object[]> _data = new List<object[]>
    //{
    //    new object[] {5, 1, 3, 9},
    //    new object[] {7, 1, 5, 3}
    //};

    //    public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

    //    IEnumerator IEnumerable.GetEnumerator() => _data.GetEnumerator();
    //}

    public class UnitTest1
    {
        private UndirectedGraph<int, IEdge<int>> Clique(int vertices)
        {
            var g = new UndirectedGraph<int, IEdge<int>>();
            for (int i = 0; i < vertices; i++)
            {
                g.AddVertex(i);
                for(int j = 0; j < i; j++)
                {
                    g.AddEdge(new Edge<int>(j, i));
                }
            }

            return g;
        }

        [Fact]
        public void TriangleIs3Colorable()
        {
            var BFFinder = new BruteForceColouringFinder();
            var clique3 = Clique(3);
            var coloring = BFFinder.Find3Colorings(clique3);
            var isColoringCorrect = ColoringChecker.CheckColoring(clique3, coloring);
            Assert.True(isColoringCorrect);
        }
        [Fact]
        public void FourCliqueIsNot3Colorable()
        {
            var BFFinder = new BruteForceColouringFinder();
            var clique4 = Clique(4);

            var coloring = BFFinder.Find3Colorings(clique4);
            Assert.Null(coloring);
        }


    }
}
