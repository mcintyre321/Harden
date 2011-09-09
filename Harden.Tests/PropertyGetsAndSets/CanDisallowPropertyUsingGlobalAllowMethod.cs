using System.Reflection;
using NUnit.Framework;

namespace Harden.Tests.PropertyGetsAndSets
{
    [TestFixture]
    public class CanDisallowPropertyUsingGlobalAllowMethod
    {
        public class Pansy
        {
            public virtual string Fears { get; set; }
            public virtual string Hobby { get; set; }

            public virtual bool Allow(MethodInfo methodInfo)
            {
                if (methodInfo.Name == "set_Fears" || methodInfo.Name == "get_Fears")
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
        public void CanSetAllowedProperty()
        {
            var pansy = Hardener.Harden(new Pansy());
            pansy.Hobby = "Fighting";
        }
        [Test]
        public void CanDisallowSet()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.Throws<HardenException>(() => pansy.Fears = "The dark");
        }
        [Test]
        public void CanDisallowGet()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.Throws<HardenException>(() => Assert.IsNull(pansy.Fears));
        }
    }
}