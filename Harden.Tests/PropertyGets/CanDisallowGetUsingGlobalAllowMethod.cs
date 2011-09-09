using System.Reflection;
using NUnit.Framework;

namespace Harden.Tests.PropertyGets
{
    [TestFixture]
    public class CanDisallowGetUsingGlobalAllowMethod
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

            public virtual string Hobby
            {
                get
                {
                    return "Fighting";
                }
            }

            public virtual bool Allow(MethodInfo methodInfo)
            {
                if (methodInfo.Name == "get_Fears")
                    return false;
                return true;
            }
        }

        [Test]
        public void CanGetAllowedProperty()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.AreEqual("Fighting", pansy.Hobby);
        }

        [Test]
        public void CanDisallowMethodCall()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.Throws<HardenException>(() => Assert.IsNotEmpty(pansy.Fears));
        }
    }
}