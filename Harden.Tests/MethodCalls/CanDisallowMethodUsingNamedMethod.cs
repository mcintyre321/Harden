using NUnit.Framework;

namespace Harden.Tests.MethodCalls
{
    [TestFixture]
    public class CanDisallowMethodXUsingNamedMethod
    {
        public class Pansy
        {
            public virtual bool Grow()
            {
                return true;
            }

            public virtual void Wilt()
            {
            }

            public virtual bool AllowWilt()
            {
                return false;
            }
        }

        [Test]
        public void CanCallAllowedMethod()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.True(pansy.Grow());
        }

        [Test]
        public void CanDisallowMethodCall()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.Throws<HardenException>(() => pansy.Wilt());
        }
    }
}