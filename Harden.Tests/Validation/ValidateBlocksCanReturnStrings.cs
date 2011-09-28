using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harden.Tests.Validation;
using NUnit.Framework;

namespace Harden.Tests.Validation
{
    [TestFixture]
    public class ValidateBlocksCanReturnStrings
    {
        public class Pansy
        {
            public virtual string Hobby { get; set; }

            public virtual IEnumerable<string> ValidateHobby(string value)
            {
                if (value == "Picking flowers") yield return "Picking flowers isn't hard!";
            }

        }

        [Test]
        public void ValidationErrorsAreReturned()
        {
            var pansy = new Pansy().Harden();
            var ve = Assert.Throws<ValidationException>(() => pansy.Hobby = "Picking flowers");
            Assert.AreEqual(1, ve.Errors.Count());
            Assert.AreEqual(pansy, ve.Object);
            Assert.AreEqual("Picking flowers isn't hard!", ve.Errors.Single().Message);
            Assert.AreEqual("Hobby", ve.Errors.Single().Field);
        }

        [Test]
        public void CanCallWithValidData()
        {
            var pansy = Hardener.Harden(new Pansy());
            pansy.Hobby = "Fighting";
        }
    }

   
    
}

