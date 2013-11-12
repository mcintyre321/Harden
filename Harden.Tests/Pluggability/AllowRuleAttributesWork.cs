using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Harden.Tests.Pluggability
{
    [TestFixture]
    public class AllowRuleAttributesWork
    {
        public class Pansy
        {
            [DisallowRule()]
            public virtual string Weaknesses()
            {
                return "Peanut allergy";
            }

            [DisallowRule()]
            public virtual string Weakness
            {
                get
                {
                    return "Peanut allergy";
                }
            }
        }


        [Test]
        public void CanNotCallDisallowedMethod()
        {

            var pansy = Hardener.Create(() => new Pansy());
            Assert.Throws<HardenException>(() => pansy.Weaknesses());
        }

        [Test]
        public void CanNotCallDisallowedProperty()
        {

            var pansy = Hardener.Create(() => new Pansy());
            string x;
            Assert.Throws<HardenException>(() => x = pansy.Weakness);
        }

    }

    public class DisallowRule : Attribute, IAllowRule
    {
        public bool? Allow(object o, MethodInfo methodbeingcalled)
        {
            return false;
        }
    }
}
