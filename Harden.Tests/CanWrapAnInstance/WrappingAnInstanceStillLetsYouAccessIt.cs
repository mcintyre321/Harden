using System.Reflection;
using NUnit.Framework;

namespace Harden.Tests.MethodCalls
{
    [TestFixture]
    public class WrappingAnInstanceStillLetsYouAccessIt
    {
        public class Pansy
        {
            private string _value;
            public Pansy()
            {
            }

            public virtual string Value { get { return _value; } set { _value = value; }}
        }

        [Test]
        public void CanCallAllowedMethod()
        {
            var pansy = new Pansy() {Value = "Hello"};
            pansy = Hardener.Harden(pansy);
            Assert.AreEqual("Hello", pansy.Value);
        }
    }
}