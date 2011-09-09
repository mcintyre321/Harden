using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harden.Tests.Validation;
using NUnit.Framework;

namespace Harden.Tests.Validation
{
    [TestFixture]
    public class CanValidateMethodParameters
    {
        public class Pansy
        {
            private string hobby;

            public virtual void SetHobby(string ignore, string hobby, string ignoreMeToo) //the ignore parameters are there to show that you don't need to validate all paramters
            {
                this.hobby = hobby;
            }

            public virtual IEnumerable<Error> ValidateSetHobby(string hobby)
            {
                if (hobby == "Picking flowers") yield return new Error("hobby", "Picking flowers is not hard!");
            }

        }

        [Test]
        public void ValidationErrorsAreReturned()
        {
            var pansy = Hardener.Harden(new Pansy());
            var ve = Assert.Throws<ValidationException>(() => pansy.SetHobby(null, "Picking flowers", null));
            Assert.AreEqual(1, ve.Errors.Count());
            Assert.AreEqual(pansy, ve.Object);
            Assert.AreEqual("Picking flowers is not hard!", ve.Errors.Single().Message);
            Assert.AreEqual("hobby", ve.Errors.Single().Field);
        }

        [Test]
        public void CanCallWithValidData()
        {
            var pansy = Hardener.Harden(new Pansy());
            pansy.SetHobby(null, "Fighting", null);
        }
    }

   
    
}

