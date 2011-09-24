using System;
using NUnit.Framework;

namespace Harden.Tests.AllowsExtension
{
    [TestFixture]
    public class AllowsExtensionActuallyWorks
    {
        public class Pansy
        {
            public virtual void SomeMethod()
            {
            }
            public bool AllowSomeMethod()
            {
                return false;
            }
            public virtual string SomeMethod2()
            {
                return "some return value";
            }
            public bool AllowSomeMethod2()
            {
                return false;
            }

            public virtual string SomeProperty
            {
                get;
                set;
            }

            public bool AllowSomeProperty()
            {
                return false;
            }

            public virtual string SomeProperty2
            {
                get;
                set;
            }

            public bool AllowSomeProperty2()
            {
                return true;
            }


        }

        [Test]
        public void WorksWithMethodCallByName()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(pansy.Allow("SomeMethod"));
        }

        //[Test]
        //public void WorksWithMethodCallByLambda()
        //{
        //    var pansy = Hardener.Harden(new Pansy());
        //    Assert.IsFalse(Allow.Call(() => pansy.SomeMethod()));
        //}
        //[Test]
        //public void WorksWithMethodWithReturnTypeCallByLambda()
        //{
        //    var pansy = Hardener.Harden(new Pansy());
        //    Assert.IsFalse(Allow.Call(() => pansy.SomeMethod2()));
        //}

        [Test]
        public void WorksWithPropertyCallToSetByName()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(pansy.Allow("get_SomeProperty"));
        }
        [Test]
        public void WorksWithPropertyCallToGetByName()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(pansy.Allow("set_SomeProperty"));
        }

        [Test]
        public void WorksWithPropertyGetByName()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(pansy.AllowGet("SomeProperty"));
        }

        [Test]
        public void WorksWithPropertySetByName()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(pansy.AllowSet("SomeProperty"));
        }

        [Test]
        public void WorksWithPropertyGetByLambda()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(Allow.Get(() => pansy.SomeProperty));
            ;
        }

        [Test]
        public void WorksWithPropertySetByLambda()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(Allow.Set(() => pansy.SomeProperty));
        }
    }

   
}
