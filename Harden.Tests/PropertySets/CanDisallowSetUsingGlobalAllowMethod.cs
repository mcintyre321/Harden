using System.Reflection;
using NUnit.Framework;

namespace Harden.Tests.PropertySets
{
    [TestFixture]
    public class CanDisallowSetUsingGlobalAllowMethod
    {
        public class Pansy
        {
            public virtual string Fears { get; set; }
            public virtual string Hobby { get; set; }

            public virtual bool Allow(MethodInfo methodInfo)
            {
                if (methodInfo.Name == "set_Fears")
                    return false;
                return true;
            }
        }

        [Test]
        public void CanGetAllowedProperty()
        {
            var pansy = Hardener.Harden(new Pansy());
            pansy.Hobby = "Fighting";
            Assert.AreEqual("Fighting", pansy.Hobby);
        }

        [Test]
        public void CanDisallowSet()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsNull(pansy.Fears);
            Assert.Throws<HardenException>(() => pansy.Fears = "The dark");
        }
        [Test]
        public void CanGetPropertyWithDisallowedSet()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsNullOrEmpty(pansy.Fears);
        }
    }
}