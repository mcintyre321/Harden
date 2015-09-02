using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Harden.Tests.Validation;
using NUnit.Framework;

namespace Harden.Tests.Validation
{
    [TestFixture]
    public class GlobalAllowsDoNotBlockValidateMethods
    {
        public class Pansy
        {
            public virtual string Hobby { get; set; }

            public virtual IEnumerable<Error> ValidateHobby(string value)
            {
                if (value == "Picking flowers") yield return new Error("Picking flowers isn't hard!");
            }

            public virtual bool Allow(MethodInfo mi)
            {
                return (mi.IsProperty());
            }

        }

        [Test]
        public void ValidationErrorsAreReturnedEvenThoughMethodsAreDisallowed()
        {
            var pansy = Hardener.Create<Pansy>(Allow.Allower);
            Assert.Throws<ValidationException>(() => pansy.Hobby = "Picking flowers");
        }

        [Test]
        public void CanCallWithValidData()
        {
            var pansy = Hardener.Create<Pansy>(Allow.Allower);
            pansy.Hobby = "Fighting";
        }
    }

   
    
}

