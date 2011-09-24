using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Harden.Tests.Pluggability
{
    [TestFixture]
    public class CanPlugARuleIntoAllowProcess
    {
        public class Pansy
        {
            [NotAllowedByDynamicRule]
            public virtual string Weaknesses()
            {
                return "Peanut allergy";
            }
        }


        [Test]
        public void CanNotCallDisallowedMethod()
        {
            HardenExtensions.Rules.Add(NotAllowedByDynamicRuleAttribute.Rule);
            var pansy = Hardener.Create(() => new Pansy());
            NotAllowedByDynamicRuleAttribute.Allow = false;
            Assert.Throws<HardenException>(() => pansy.Weaknesses());
        }

        [Test]
        public void CanCallAllowedMethod()
        {
            HardenExtensions.Rules.Add(NotAllowedByDynamicRuleAttribute.Rule);
            var pansy = Hardener.Create(() => new Pansy());
            NotAllowedByDynamicRuleAttribute.Allow = true;
            Assert.AreEqual("Peanut allergy", pansy.Weaknesses());
        }
    }
    
    internal class NotAllowedByDynamicRuleAttribute : Attribute
    {
        public static bool? Allow;
        public readonly static Func<dynamic, MethodInfo, bool?> Rule = (o, mi) =>
        {
            var att = mi.GetCustomAttributes(typeof(NotAllowedByDynamicRuleAttribute), true).Cast<NotAllowedByDynamicRuleAttribute>().SingleOrDefault();
            if (att != null)
            {
                return Allow;
            }
            return null;
        };
    }
}
