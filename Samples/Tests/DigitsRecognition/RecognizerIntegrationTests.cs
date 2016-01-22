using System;
using DigitsRecognition;
using Ninject;
using Xunit;
using FluentAssertions;

namespace Tests.DigitsRecognition
{
    public class RecognizerIntegrationTests
    {
        [Fact (Skip = "")]
        public void should_train_and_evaluate_model()
        {
            const string trainingSetPath = @"C:\Data\Repos\Data\trainingsample.csv";
            const string testSetPath = @"C:\Data\Repos\Data\validationsample.csv";
            var kernel = new StandardKernel(new DigitsRecognitionModule());
            var recognizer = kernel.Get<Recognizer>();
            var result = recognizer.RecogniseAndEvaluate(trainingSetPath, testSetPath);
            result.Should().BeGreaterThan(0.0);
            Console.Out.WriteLine(result);
        } 
    }
}