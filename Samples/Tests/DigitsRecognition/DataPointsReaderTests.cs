using System.Collections.Generic;
using DigitsRecognition;
using FluentAssertions;
using Xunit;


namespace Tests.DigitsRecognition
{
    public class DataPointsReaderTests
    {
        [Fact]
        public void should_read_data_points_from_file()
        {
            var result = new DataPointsReader().ReadPoints(@"./DigitsRecognition/TestData/testPixels.csv");
            result.Should().BeEquivalentTo(new List<DataPoint>
            {
                new DataPoint {Label = "1", Pixels = new List<int> {1, 0, 0, 0}},
                new DataPoint {Label = "0", Pixels = new List<int> {0, 1, 0, 0}},
                new DataPoint {Label = "1", Pixels = new List<int> {0, 0, 1, 0}},
                new DataPoint {Label = "4", Pixels = new List<int> {0, 0, 0, 1}},
            });
        }
    }
}