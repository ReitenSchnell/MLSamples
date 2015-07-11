using System.Collections.Generic;
using DigitsRecognition;
using NSubstitute;
using Xunit.Extensions;
using FluentAssertions;

namespace Tests.DigitsRecognition
{
    public class EvaluatorTests
    {
        private readonly IClassifier classifier = Substitute.For<IClassifier>();
        private readonly Evaluator evaluator;

        public EvaluatorTests()
        {
            evaluator = new Evaluator(classifier);
        }

        [Theory, NSubData(4)]
        public void should_return_percentage_of_rigth_anwers(List<DataPoint> dataPoints)
        {
            classifier.Predict(dataPoints[0].Pixels).Returns(dataPoints[0].Label);
            classifier.Predict(dataPoints[1].Pixels).Returns("foo");
            classifier.Predict(dataPoints[2].Pixels).Returns(dataPoints[2].Label);
            classifier.Predict(dataPoints[3].Pixels).Returns("boo");

            var result = evaluator.Evaluate(dataPoints);

            result.Should().Be(0.5);
        }
    }
}