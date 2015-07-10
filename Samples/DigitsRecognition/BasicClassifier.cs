using System.Collections.Generic;
using System.Linq;

namespace DigitsRecognition
{
    public class BasicClassifier : IClassifier
    {
        private readonly IDistance distance;
        private List<DataPoint> trainingSet; 

        public BasicClassifier(IDistance distance)
        {
            this.distance = distance;
        }

        public void Train(List<DataPoint> points)
        {
            trainingSet = points;
        }

        public string Predict(List<int> pixels)
        {
            return
                trainingSet.Select(point => new {point.Label, Distance = distance.Between(point.Pixels, pixels)})
                    .OrderBy(arg => arg.Distance)
                    .First()
                    .Label;
        }
    }
}