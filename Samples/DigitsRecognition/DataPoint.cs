using System.Collections.Generic;
using System.Linq;

namespace DigitsRecognition
{
    public class DataPoint
    {
        public string Label { get; set; }
        public List<int> Pixels  { get; set; }

        protected bool Equals(DataPoint other)
        {
            return Pixels.SequenceEqual(other.Pixels) && string.Equals(Label, other.Label);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataPoint) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Pixels != null ? Pixels.GetHashCode() : 0)*397) ^ (Label != null ? Label.GetHashCode() : 0);
            }
        }
    }
}
