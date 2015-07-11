using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Xunit;

namespace Tests
{
    public class NSubDataAttribute : AutoDataAttribute
    {
        public NSubDataAttribute() :
            base(new Fixture())
        {
        }

        public NSubDataAttribute(int repeatCount)
            : this()
        {
            Fixture.RepeatCount = repeatCount;
        }
    }
}