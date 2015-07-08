using System;
using System.Collections.Generic;
using System.Linq;

namespace DigitsRecognition
{
    public class ManhattanDistance : IDistance
    {
        public double Between(List<int> image1, List<int> image2)
        {
            if (image1.Count != image2.Count)
                throw new ArgumentException("Images have different image sizes");
            return image1.Zip(image2, (i1, i2) => Math.Abs(i1 - i2)).Sum();
        }
    }
}