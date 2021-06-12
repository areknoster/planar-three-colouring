using System;
using System.Collections.Generic;
using System.Linq;
using Planar3Coloring.GrahGenerator;
using Xunit;

namespace Planar3Coloring.Test
{
    public class GraphGeneratorTests
    {
        [Fact]
        public void CorrectDivisionOfFaceMultipleConnections()
        {
            var face = new Face(new Random(), new List<int>()
            {
                0, 1, 2, 3, 4
            });
            var connections = new HashSet<int>() {0, 2, 4};
            var faces = face.DivideByConnections(5, connections);
            Assert.Contains(faces, f => f.Vertices.SequenceEqual(new List<int>{5,0,1,2}));
            Assert.Contains(faces, f => f.Vertices.SequenceEqual(new List<int>{5,2,3,4}));
            Assert.Contains(faces, f => f.Vertices.SequenceEqual(new List<int>{5,4,0}));

        }
        [Fact]
        public void CorrectDivisionOfFaceSingleConnection()
        {
            var face = new Face(new Random(), new List<int>()
            {
                3,0,1,2
            });
            var connections = new HashSet<int>() {1};
            var faces = face.DivideByConnections(4, connections);
            Assert.Single(faces);
            Assert.Contains(faces, f => f.Vertices.SequenceEqual(new List<int>{4,1,2,3,0,1}));

        }
    }
}