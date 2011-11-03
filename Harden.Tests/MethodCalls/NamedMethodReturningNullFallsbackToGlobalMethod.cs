using System.Reflection;
using NUnit.Framework;

namespace Harden.Tests.MethodCalls
{
    [TestFixture]
    public class NamedMethodOverridesGlobalMethod
    {
        public class Pansy
        {
            public virtual bool Grow()
            {
                return true;
            }

            public virtual bool AllowGrow()
            {
                return true;
            }

            public virtual void Wilt()
            {
            }

            

            public virtual bool Allow(MethodInfo methodInfo)
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
            Assert.Throws<HardenException>(pansy.Wilt);
        }
    }
}