using System.Collections.Generic;
using System.Linq;

namespace DigitsRecognition
{
    public interface IEvaluator
    {
        double Evaluate(List<DataPoint> validationSet);
    }

    public class Evaluator : IEvaluator
    {
        private readonly IClassifier classifier;

        public Evaluator(IClassifier classifier)
        {
            this.classifier = classifier;
        }

        public double Evaluate(List<DataPoint> validationSet)
        {
            return validationSet.Select(Score).Average();
        }

        private double Score(DataPoint dataPoint)
        {
            var label = classifier.Predict(dataPoint.Pixels);
            return label == dataPoint.Label ? 1.0 : 0.0;
        }
    }
}