using System.Collections;
using System.Collections.Generic;

namespace DigitsRecognition
{
    public interface IClassifier
    {
        void Train(List<DataPoint> points);
        string Predict(List<int> pixels);
    }
}