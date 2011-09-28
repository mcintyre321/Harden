using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harden.ValidationAttributes;

namespace Harden
{
    public static class Validation
    {
        public delegate IEnumerable<Tuple<string, string>> ValidationRule(dynamic obj, MethodInfo mi, object[] parameters);
        public static IList<ValidationRule> Rules;
        static Validation()
        {
            Rules = new List<ValidationRule>();
            Rules.AddFor<NotNullAttribute>(NotNullAttribute.ValidationRule);
            Rules.AddFor<LengthAttribute>(LengthAttribute.ValidationRule);

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
                    var validationErrorsObj = validateMethod.Invoke(obj, args);
                    var validationErrors = validationErrorsObj as IEnumerable<Error>;
                    if (validationErrors == null)
                    {
                        var validationStrings = validationErrorsObj as IEnumerable<string>;
                        if (validationStrings != null)
                        {
                            validationErrors = validationStrings.Select(s => new Error(s));
                        }
                    }
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

    //internal static class ValidatioRuleConfigExtensions
    //{
    //    public static void AddFor<TAttribute>(this IList<Validation.ValidationRule> rules, AttributeRuleImpl<TAttribute> rule)
    //    {
    //        rules.Add(MakeRule<TAttribute>(rule));
    //    }

    //    public delegate IEnumerable<Tuple<string, string>> AttributeRuleImpl<TAtt>(TAtt att, object o, MethodInfo mi, object[] parameters);
    //    static Validation.ValidationRule MakeRule<TAttribute>(AttributeRuleImpl<TAttribute> rule)
    //    {
    //        return (obj, mi, args) => AttributeValidationRule(rule, obj, mi, args);
    //    }
    //    private static IEnumerable<Tuple<string, string>> AttributeValidationRule<TAttribute>(AttributeRuleImpl<TAttribute> rule, dynamic obj, MethodInfo mi, object[] args)
    //    {
    //        var isProperty = mi.Name.StartsWith("set_");
    //        var name = isProperty ? mi.Name.Substring(4) : mi.Name;
    //        var atts = isProperty
    //                       ? mi.DeclaringType.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).GetCustomAttributes(typeof(TAttribute), true)
    //                       : mi.GetCustomAttributes(typeof(TAttribute), true);

    //        if (atts != null && atts.Any())
    //        {
    //            var att = (TAttribute)atts.Single();
    //            if (att != null)
    //            {
    //                foreach (var validationErrorMessage in rule(att, obj, mi, args))
    //                {
    //                    yield return validationErrorMessage;
    //                }
    //            }
    //        }
    //    }
    //}

}
