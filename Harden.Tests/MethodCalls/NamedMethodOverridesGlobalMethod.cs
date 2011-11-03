using System.Reflection;
using NUnit.Framework;

namespace Harden.Tests.MethodCalls
{
    [TestFixture]
    public class NamedMethodReturningNullFallsBackToGlobalMethod
    {
        public class Pansy
        {
            public virtual void Grow()
            {
            }

            public virtual bool? AllowGrow()
            {
                return null;
            }
 
            public virtual bool Allow(MethodInfo methodInfo)
            {
                return false;
            }
        }

        [Test]
        public void CanDisallowMethodCall()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.Throws<HardenException>(pansy.Grow);
        }
    }
}