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
    public class ValidationAttributesWork
    {
        public class Pansy
        {
            [NotNull] [Length(1, 50)]
            public virtual string SomeProperty { get; set; }
        }

        [Test]
        public void NotNullWorks()
        {
            //Harden.Validation.Rules.Add(ValidateRequiredAttribute);
            var pansy = Hardener.Create(() => new Pansy());
            pansy.SomeProperty = "Not going to throw an error";
            var ve = Assert.Throws<ValidationException>(() => pansy.SomeProperty = null);
            Assert.AreEqual(ve.Errors.Single().Field, "SomeProperty");
            Assert.AreEqual(ve.Errors.Single().Message, "Required");
        }

        [Test]
        public void LengthAllowed()
        {
            //Harden.Validation.Rules.Add(ValidateRequiredAttribute);
            var pansy = Hardener.Create(() => new Pansy());
            foreach (var i in Enumerable.Range(1, 50))
            {
                pansy.SomeProperty = new string('a', i);
            }
            var ve = Assert.Throws<ValidationException>(() => pansy.SomeProperty = null);
            Assert.AreEqual(ve.Errors.Single().Field, "SomeProperty");
        }
        [Test]
        public void MinLengthWorks()
        {
            //Harden.Validation.Rules.Add(ValidateRequiredAttribute);
            var pansy = Hardener.Create(() => new Pansy());
            var ve = Assert.Throws<ValidationException>(() => pansy.SomeProperty = "");
            Assert.AreEqual(ve.Errors.Single().Field, "SomeProperty");
        }

        [Test]
        public void MaxLengthWorks()
        {
            //Harden.Validation.Rules.Add(ValidateRequiredAttribute);
            var pansy = Hardener.Create(() => new Pansy());
            var ve = Assert.Throws<ValidationException>(() => pansy.SomeProperty = new string('a', 51));
            Assert.AreEqual(ve.Errors.Single().Field, "SomeProperty");
        }


    }
}
