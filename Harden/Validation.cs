using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Harden
{
    public static class Validation
    {
        public delegate IEnumerable<Tuple<string, string>> ValidationRule(dynamic obj, MethodInfo mi, object[] parameters);
        public static IList<ValidationRule> Rules;
        static Validation()
        {
            Rules = new List<ValidationRule>();
        }

        internal static void Execute(object obj, MethodInfo mi, object[] arguments)
        {
            if (arguments.Length > 0) // we may need to validate these arguments...
            {
                var errors = new List<Error>();
                string name = mi.Name;
                if (name.StartsWith("set_"))
                {
                    name = name.Substring(4);
                }
                var validateMethod = obj.GetType().GetMethod("Validate" + name, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.NonPublic | BindingFlags.Public);
                if (validateMethod != null)
                {
                    object[] args = null;

                    if (arguments.Length > 1)
                    {
                        var argsQ = from p in validateMethod.GetParameters()
                                    from a in arguments.Zip(mi.GetParameters(), (arg, pi) => new { pi.Name, arg })
                                    where p.Name == a.Name
                                    select a.arg;
                        args = argsQ.ToArray();
                    }
                    else
                    {
                        args = arguments;
                    }
                    var validationErrors = validateMethod.Invoke(obj, args) as IEnumerable<Error>;
                    if (validationErrors != null) errors.AddRange(validationErrors);
                }

                foreach (var validationRule in Rules)
                {
                    var validationErrors = validationRule(obj, mi, arguments).Select(e => new Error(e.Item1, e.Item2));
                    if (validationErrors != null) errors.AddRange(validationErrors);
                }

                if (errors.Any())
                {
                    if (mi.Name.StartsWith("set_"))
                    {
                        foreach (var validationError in errors)
                        {
                            validationError.Field = name;
                        }
                    }
                    throw new ValidationException(obj, errors);
                }
            }
        }
    }
}
