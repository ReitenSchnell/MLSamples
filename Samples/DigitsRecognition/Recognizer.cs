using System;
using System.IO;

namespace DigitsRecognition
{
    public class Recognizer
    {
        private readonly IDataPointsReader dataPointsReader;
        private readonly IClassifier classifier;
        private readonly IEvaluator evaluator;

        public Recognizer(IDataPointsReader dataPointsReader, IClassifier classifier, IEvaluator evaluator)
        {
            this.dataPointsReader = dataPointsReader;
            this.classifier = classifier;
            this.evaluator = evaluator;
        }

        public double RecogniseAndEvaluate(string trainingSetPath, string testSetPath)
        {
            var trainingSet = dataPointsReader.ReadPoints(trainingSetPath);
            Console.Out.WriteLine("Training set contains {0} data points", trainingSet.Count);
            var testSet = dataPointsReader.ReadPoints(testSetPath);
            Console.Out.WriteLine("Test set contains {0} data points", testSet.Count);
            classifier.Train(trainingSet);
            return evaluator.Evaluate(testSet);
        }
    }
}