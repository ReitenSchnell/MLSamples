using System.Collections.Generic;

namespace DigitsRecognition
{
    public interface IDistance
    {
        double Between(List<int> image1, List<int> image2);
    }
}