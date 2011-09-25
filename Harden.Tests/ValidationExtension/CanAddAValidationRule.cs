using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Harden.Tests.ValidationExtension
{
    [TestFixture]
    public class CanAddAValidationRule
    {
        public class Pansy
        {
            [Required]
            public virtual string SomeProperty { get; set; }
        }

        [Test]
        public void WorksWithMethodCallByName()
        {
            Harden.Validation.Rules.Add(ValidateRequiredAttribute);
            var pansy = Hardener.Create(() => new Pansy());
            pansy.SomeProperty = "Not going to throw an error";
            var ve = Assert.Throws<ValidationException>(() => pansy.SomeProperty = null);
            Assert.AreEqual(ve.Errors.Single().Field, "SomeProperty");
            Assert.AreEqual(ve.Errors.Single().Message, "Required");

        }

        private static IEnumerable<Tuple<string, string>> ValidateRequiredAttribute(dynamic obj, MethodInfo mi, object[] arguments)
        {
            var isProperty = mi.Name.StartsWith("set_");
            var name = isProperty ? mi.Name.Substring(4) : mi.Name;
            var atts = isProperty
                               ? mi.DeclaringType.GetProperty(name).GetCustomAttributes(typeof(RequiredAttribute), true)
                               : mi.GetCustomAttributes(typeof(RequiredAttribute), true);
            if (atts.Any())
            {
                if (arguments.Single() == null)
                {
                    yield return Tuple.Create(name, "Required");
                }
            }
        }


    }
}
