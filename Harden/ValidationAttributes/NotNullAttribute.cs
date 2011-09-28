using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Harden.ValidationAttributes
{
    public class NotNullAttribute : Attribute
    {
        public static IEnumerable<Tuple<string, string>> ValidationRule(NotNullAttribute att, object o, MethodInfo mi, object[] parameters)
        {
            var param = parameters.SingleOrDefault();
            if (param == null) yield return Tuple.Create(mi.Name, "Required");
        }
    }
}