using System.Collections.Generic;
using DigitsRecognition;
using FluentAssertions;
using NSubstitute;
using Ploeh.AutoFixture.Xunit;
using Xunit.Extensions;

namespace Tests.DigitsRecognition
{
    public class BasicClassifierTests
    {
        private readonly IDistance distance = Substitute.For<IDistance>();
        private readonly BasicClassifier classifier;

        public BasicClassifierTests()
        {
            classifier = new BasicClassifier(distance);
        }


        [Theory, AutoData]
        public void should_predict_nearest_label(List<DataPoint> dataPoints, List<int> pixels)
        {
            classifier.Train(dataPoints);
            distance.Between(dataPoints[0].Pixels, pixels).Returns(3);
            distance.Between(dataPoints[1].Pixels, pixels).Returns(1);
            distance.Between(dataPoints[2].Pixels, pixels).Returns(8);

            var result = classifier.Predict(pixels);

            result.Should().Be(dataPoints[1].Label);
        }
    }
}