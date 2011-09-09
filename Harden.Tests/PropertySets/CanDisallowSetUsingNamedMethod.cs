using NUnit.Framework;

namespace Harden.Tests.PropertySets
{
    [TestFixture]
    public class CanDisallowSetUsingNamedMethod
    {
        public class Pansy
        {
            public virtual string Fears { get; set; }

            public virtual bool AllowSetFears()
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
        public void CanGetAllowedProperty()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.AreEqual("Fighting", pansy.Hobby);
        }
        [Test]
        public void CanGetDisallowedProperty()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.Null(pansy.Fears);
        }
        

        [Test]
        public void CannotSetDisallowedProperty()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.Throws<HardenException>(() => pansy.Fears = "The Dark");
        }
    }
}