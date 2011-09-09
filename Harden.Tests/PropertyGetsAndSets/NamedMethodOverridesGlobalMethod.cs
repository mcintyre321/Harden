using NUnit.Framework;

namespace Harden.Tests.PropertyGetsAndSets
{
    [TestFixture]
    public class NamedMethodOverridesGlobalMethod
    {
        public class Pansy
        {
            public virtual string Fears
            {
                get
                {
                    return "The dark";
                }
            }

            public virtual bool AllowFears()
            {
                return false;
            }

            public virtual string Hobby
            {
                get
                {
                    return "Fighting";
                }
            }
        }

        [Test]
        public void CanCallAllowedMethod()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.AreEqual("Fighting", pansy.Hobby);
        }

        [Test]
        public void CanDisallowMethodCall()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.Throws<HardenException>(() => Assert.IsNotNull(pansy.Fears));
        }
    }
}