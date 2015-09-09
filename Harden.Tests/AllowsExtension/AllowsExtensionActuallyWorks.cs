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

            public virtual string SomeProperty2  { get; set; }
            public bool AllowSomeProperty2() { return true; }

            public virtual string MethodUsingContext()
            {
                return "hello";
            }
            public bool AllowMethodUsingContext(object context) { return context as bool? ?? false; }


        }

        [Test]
        public void WorksWithMethodCallByName()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(pansy.AllowCall("SomeMethod", null));
        }
        [Test]
        public void WorksWithContextVariable()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsTrue(pansy.AllowCall(nameof(Pansy.MethodUsingContext), true));
            Assert.IsFalse(pansy.AllowCall(nameof(Pansy.MethodUsingContext), false));
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
            Assert.IsFalse(pansy.AllowCall("get_SomeProperty", null));
        }
        [Test]
        public void WorksWithPropertyCallToGetByName()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(pansy.AllowCall("set_SomeProperty", null));
        }

        [Test]
        public void WorksWithPropertyGetByName()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(pansy.AllowGet("SomeProperty", null));
        }

        [Test]
        public void WorksWithPropertySetByName()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(pansy.AllowSet("SomeProperty", null));
        }

        [Test]
        public void WorksWithPropertyGetByLambda()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(Allow.Get(() => pansy.SomeProperty, null));
            ;
        }

        [Test]
        public void WorksWithPropertySetByLambda()
        {
            var pansy = Hardener.Harden(new Pansy());
            Assert.IsFalse(Allow.Set(() => pansy.SomeProperty, null));
        }
    }

   
}
