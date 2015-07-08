using System;
using System.Collections.Generic;
using System.Linq;
using DigitsRecognition;
using FluentAssertions;
using Xunit;

namespace Tests.DigitsRecognition
{
    public class ManhattanDistanceTests
    {
        private readonly ManhattanDistance distance = new ManhattanDistance();

        [Fact]
        public void should_throw_when_images_have_different_sizes()
        {
            var image1 = Enumerable.Range(1, 3).Select(i => i).ToList();
            var image2 = Enumerable.Range(1, 5).Select(i => i).ToList();

            distance.Invoking(manhattanDistance => manhattanDistance.Between(image1, image2))
                .ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void should_return_distance_between_images()
        {
            var image1 = new List<int> {1, 0, 1, 0};
            var image2 = new List<int> {1, 1, 0, 0};
            
            var result = distance.Between(image1, image2);

            result.Should().Be(2);
        }
    }
}