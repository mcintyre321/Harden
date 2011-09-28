
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Harden.ValidationAttributes;
using NUnit.Framework;

namespace Harden.Tests.ValidationPluggability
{
    [TestFixture]
    public class PropertyValidationWorks
    {
        public class Pansy
        {
            public virtual string SomeProperty { get; set; }
        }

        [Test]
        public void NotNullWorks()
        {
            Harden.Validation.Rules.AddPropertyRule((Pansy p) => p.SomeProperty, ReturnErrorIfNull);
            var pansy = Hardener.Create(() => new Pansy());
            pansy.SomeProperty = "Not going to throw an error";
            var ve = Assert.Throws<ValidationException>(() => pansy.SomeProperty = null);
            Assert.AreEqual(ve.Errors.Single().Field, "SomeProperty");
            Assert.AreEqual(ve.Errors.Single().Message, "Required");
        }

        private IEnumerable<Tuple<string, string>> ReturnErrorIfNull(object o, PropertyInfo pi, string value)
        {
            if (value == null) yield return Tuple.Create(pi.Name, "Required");

        }

    }
}
