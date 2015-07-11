using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DigitsRecognition
{
    public interface IDataPointsReader
    {
        List<DataPoint> ReadPoints(string path);
    }

    public class DataPointsReader : IDataPointsReader
    {
        public List<DataPoint> ReadPoints(string path)
        {
            return File.ReadAllLines(path).Skip(1).Select(ExtractPoint).ToList();
        }

        private static DataPoint ExtractPoint(string s)
        {
            var separated = s.Split(',');
            return new DataPoint{
                Label = separated.First(), 
                Pixels = separated.Skip(1).Select(str => Int32.Parse(str)).ToList()};
        }
    }
}