using System.Reflection;
using NUnit.Framework;

namespace Harden.Tests.PropertySets
{
    [TestFixture]
    public class NamedMethodOverridesGlobalMethod
    {
        public class Pansy
        {
            public virtual string Fears { get; set; }

            public virtual bool Allow(MethodInfo methodInfo)
            {
                if (methodInfo.Name.StartsWith("set_"))
                    return false;
                return true;
            }

            public virtual bool AllowSetHobby()
            {
                return true;
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
            Assert.Throws<HardenException>(() => pansy.Fears = "The Dark");
        }
    }
}