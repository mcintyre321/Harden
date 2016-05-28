using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Harden
{
    public static class ValidationRuleConfigExtensions
    {
        public static PropertyInfo GetPropInf<T, TProp>(Expression<Func<T, TProp>> t)
        {
            MemberExpression memberExpression1 = t.Body as MemberExpression;
            return memberExpression1.Member as PropertyInfo;
        }

        public delegate IEnumerable<System.Tuple<string, string>> PropertyValidationRule<in T>(dynamic o, PropertyInfo pi, T value);

        public delegate IEnumerable<System.Tuple<string, string>> PropertyValidationRule<in T, in TProp>(T o, PropertyInfo pi, TProp value);

        public static void AddPropertyRule<T, TProp>(this IList<Validation.ValidationRule> rules, Expression<Func<T, TProp>> get, PropertyValidationRule<TProp> rule)
        {
            var prop = GetPropInf(get);
            AddPropertyRule(rules, prop, (o, pi, v) => rule(o, pi, ((TProp) v)));
        }
        public static void AddPropertyRule(this IList<Validation.ValidationRule> rules, PropertyInfo pi, PropertyValidationRule<object> rule)
        {
            rules.Add((o, m, a) =>
            {
                if (m == pi.SetMethod)
                {
                    return rule(o, pi, a[0]);
                }
                else
                {
                    return new System.Tuple<string, string>[] { };
                }
            });
        }

        public static IEnumerable<System.Tuple<string, string>> ToErrors(this IEnumerable<string> errors, PropertyInfo pi)
        {
            return errors.Where(e => e != null).Select(e => Tuple.Create(pi.Name, e));
        }

        public static IEnumerable<T> Append<T>(this T t, T t2)
        {
            yield return t;
            yield return t2;
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> ts, T t2)
        {
            foreach (var t in ts)
            {
                yield return t;
            }
            yield return t2;
        }

        public static void AddFor<TAttribute>(this IList<Validation.ValidationRule> rules, AttributeRuleImpl<TAttribute> rule)
        {
            rules.Add(MakeRule(rule));
        }

        public delegate IEnumerable<Tuple<string, string>> AttributeRuleImpl<TAtt>(TAtt att, object o, MethodInfo mi, object[] parameters);
        static Validation.ValidationRule MakeRule<TAttribute>(AttributeRuleImpl<TAttribute> rule)
        {
            return (obj, mi, args) => AttributeValidationRule(rule, obj, mi, args);
        }
        private static IEnumerable<Tuple<string, string>> AttributeValidationRule<TAttribute>(AttributeRuleImpl<TAttribute> rule, dynamic obj, MethodInfo mi, object[] args)
            where TAttribute : Attribute
        {
            var isProperty = mi.Name.StartsWith("set_");
            var name = isProperty ? mi.Name.Substring(4) : mi.Name;
            var atts = isProperty
                           ? mi.DeclaringType.GetTypeInfo().GetDeclaredProperty(name).GetCustomAttributes(typeof(TAttribute), true)
                           : mi.GetCustomAttributes(typeof(TAttribute), true);

            if (atts != null && atts.Any())
            {
                var att = (TAttribute)atts.Single();
                if (att != null)
                {
                    foreach (var validationErrorMessage in rule(att, obj, mi, args))
                    {
                        yield return validationErrorMessage;
                    }
                }
            }
        }
    }
}