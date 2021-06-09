using Planar3Coloring.Triangulation;
using QuikGraph;
using System;
using System.Collections.Generic;
using Xunit;

namespace Planar3Coloring.Test
{
    public class TriangulationTests
    {
        private ITriangulation sut = new InternalTriangulation();

        [Fact]
        public void WhenGraphWithNoVertices_ReturnsEmptyGraph()
        {
            UndirectedGraph<int, IEdge<int>> graph = new UndirectedGraph<int, IEdge<int>>(false);

            UndirectedGraph<int, IEdge<int>> g = sut.Triangulate(graph);

            Assert.True(g.IsVerticesEmpty);
        }

        [Fact]
        public void WhenRootDoesNotExist_ThrowsArgumentException()
        {
            UndirectedGraph<int, IEdge<int>> graph = new UndirectedGraph<int, IEdge<int>>(false);
            graph.AddVertex(88);

            Action action = () =>
            {
                sut.Triangulate(graph, 99);
            };

            Assert.Throws<ArgumentException>(action);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(10)]
        public void WhenNStar(int N)
        {
            UndirectedGraph<int, IEdge<int>> graph = new UndirectedGraph<int, IEdge<int>>(false);
            int root = 0;
            List<IEdge<int>> edges = new List<IEdge<int>>();
            for (int i = 1; i <= N; i++)
                edges.Add(new Edge<int>(root, i));
            graph.AddVerticesAndEdgeRange(edges);

            UndirectedGraph<int, IEdge<int>> g = sut.Triangulate(graph, root);

            foreach (IEdge<int> edge in edges)
                Assert.True(g.ContainsEdge(edge));
            for (int i = 2; i <= N; i++)
                Assert.True(g.ContainsEdge(i - 1, i));
            if (N > 1)
            {
                Assert.True(g.ContainsEdge(N, 1));
                Assert.Equal(3 * N - 3, g.EdgeCount);
            }
        }

        [Theory]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(10)]
        public void WhenNPath(int N)
        {
            UndirectedGraph<int, IEdge<int>> graph = new UndirectedGraph<int, IEdge<int>>(false);
            int root = 0;
            List<IEdge<int>> edges = new List<IEdge<int>>();
            for (int i = 0; i < N; i++)
                edges.Add(new Edge<int>(i, i + 1));
            graph.AddVerticesAndEdgeRange(edges);

            UndirectedGraph<int, IEdge<int>> g = sut.Triangulate(graph, root);

            foreach (IEdge<int> edge in edges)
                Assert.True(g.ContainsEdge(edge));
            for (int i = 2; i <= N; i++)
                Assert.True(g.ContainsEdge(root, i));
            for (int i = 3; i < N; i++)
                Assert.True(g.ContainsEdge(1, i));
        }

        [Fact]
        public void SimpleBinaryTree()
        {
            UndirectedGraph<int, IEdge<int>> graph = new UndirectedGraph<int, IEdge<int>>(false);
            int root = 0;
            List<IEdge<int>> edges = new List<IEdge<int>>();
            edges.Add(new Edge<int>(0, 1));
            edges.Add(new Edge<int>(0, 2));
            edges.Add(new Edge<int>(1, 3));
            edges.Add(new Edge<int>(1, 4));
            edges.Add(new Edge<int>(2, 5));
            edges.Add(new Edge<int>(2, 6));
            graph.AddVerticesAndEdgeRange(edges);

            UndirectedGraph<int, IEdge<int>> g = sut.Triangulate(graph, root);

            foreach (IEdge<int> edge in edges)
                Assert.True(g.ContainsEdge(edge));
            Assert.True(g.ContainsEdge(1, 2));
            Assert.True(g.ContainsEdge(0, 3));
            Assert.True(g.ContainsEdge(3, 4));
            Assert.True(g.ContainsEdge(4, 2));
            Assert.True(g.ContainsEdge(4, 5));
            Assert.True(g.ContainsEdge(5, 6));
            Assert.True(g.ContainsEdge(6, 0));
            Assert.True(g.ContainsEdge(0, 4));
            Assert.True(g.ContainsEdge(0, 5));
        }
    }
}
