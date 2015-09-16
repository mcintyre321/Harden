using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harden.ValidationAttributes
{
    public class LengthAttribute : Attribute
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public LengthAttribute(int max) : this(0, max)
        {
            
        }
        public LengthAttribute(int min, int max) 
        {
            if (min > max) throw new ArgumentException(string.Format("Min value '{0}' must be less than max Value '{1}'", min, max));
            if (min < 0) throw new ArgumentException(string.Format("Min value '{0}' must be greater or equal to zero", min, max));
            Min = min;
            Max = max;
        }

        public static IEnumerable<Tuple<string, string>> ValidationRule(LengthAttribute att, object o, MethodInfo mi, object[] parameters)
        {
            var param = parameters.SingleOrDefault();
            if (param == null) yield break;
            var stringRep = param.ToString();
            if (stringRep.Length < att.Min) yield return Tuple.Create(mi.GetParameters().Single().Name, "Must be at least " + att.Min + " characters long");
            if (stringRep.Length > att.Max) yield return Tuple.Create(mi.GetParameters().Single().Name, "Must be at most " + att.Max + " characters long");
        }
    }
}