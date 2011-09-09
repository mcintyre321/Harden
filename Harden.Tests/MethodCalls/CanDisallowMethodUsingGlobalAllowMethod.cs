using System.Reflection;
using NUnit.Framework;

namespace Harden.Tests.MethodCalls
{
    [TestFixture]
    public class CanDisallowMethodUsingGlobalAllowMethod
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

            public virtual bool Allow(MethodInfo methodInfo)
            {
                if (methodInfo.Name == "Wilt")
                    return false;
                return true;
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
            Assert.Throws<HardenException>(pansy.Wilt);
        }
    }
}