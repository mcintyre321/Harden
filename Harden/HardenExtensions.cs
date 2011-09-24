using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;

namespace Harden
{
    public static class Allow
    {
        private static Tuple<object, PropertyInfo> GetObjAndProp<T>(Expression<Func<T>> t)
        {
            var propertyGetExpression = t.Body as MemberExpression;

            // "s" is replaced by a field access on a compiler-generated class from the closure
            var fieldOnClosureExpression = propertyGetExpression.Expression as MemberExpression;

            // Find the compiler-generated class
            var closureClassExpression = fieldOnClosureExpression.Expression as ConstantExpression;
            var closureClassInstance = closureClassExpression.Value;

            // Find the field value, in this case it's a reference to the "s" variable
            var closureFieldInfo = fieldOnClosureExpression.Member as FieldInfo;
            var closureFieldValue = closureFieldInfo.GetValue(closureClassInstance);

            // We know that the Expression is a property access so we get the PropertyInfo instance
            // And even access the value (yes compiling the expression would have been simpler :D)
            var propertyInfo = propertyGetExpression.Member as PropertyInfo;
            //var propertyValue = propertyInfo.GetValue(closureFieldValue, null);

            return Tuple.Create(closureFieldValue, propertyInfo);
        }
        private static Tuple<object, MethodInfo> GetObjAndMethod<T>(Expression<Func<T>> t)
        {
            var propertyGetExpression = t.Body as MemberExpression;

            // "s" is replaced by a field access on a compiler-generated class from the closure
            var fieldOnClosureExpression = propertyGetExpression.Expression as MemberExpression;

            // Find the compiler-generated class
            var closureClassExpression = fieldOnClosureExpression.Expression as ConstantExpression;
            var closureClassInstance = closureClassExpression.Value;

            // Find the field value, in this case it's a reference to the "s" variable
            var closureFieldInfo = fieldOnClosureExpression.Member as FieldInfo;
            var closureFieldValue = closureFieldInfo.GetValue(closureClassInstance);

            // We know that the Expression is a property access so we get the PropertyInfo instance
            // And even access the value (yes compiling the expression would have been simpler :D)
            var propertyInfo = propertyGetExpression.Member as MethodInfo;
            //var propertyValue = propertyInfo.GetValue(closureFieldValue, null);

            return Tuple.Create(closureFieldValue, propertyInfo);
        }
        private static Tuple<object, MethodInfo> GetObjAndMethod<T>(Expression<Action> t)
        {
            throw new NotImplementedException();
        }

	
        public static bool Get<T>(Expression<Func<T>> t)
        {
            var tup = GetObjAndProp(t);
            return tup.Item1.Allow(tup.Item2.GetGetMethod());
        }
        public static bool Set<T>(Expression<Func<T>> t)
        {
            var tup = GetObjAndProp(t);
            return tup.Item1.Allow(tup.Item2.GetSetMethod());
        }
        //public static bool Call<T>(Expression<Func<T>> t)
        //{
        //    var tup = GetObjAndMethod(t);
        //    return tup.Item1.Allow(tup.Item2);
        //}
        //public static bool Call(Expression<Action> t)
        //{
        //    var tup = GetObjAndMethod<Expression<Action>>(t);
        //    return tup.Item1.Allow(tup.Item2);
        //}

    }
    public static class HardenExtensions
    {
        static HardenExtensions()
        {
            Rules = new List<Func<dynamic, MethodInfo, bool?>>();
        }

        public static bool Allow(this object obj, string methodName)
        {
            return Allow(obj, obj.GetType().GetMethod(methodName));
        }

        public static bool AllowSet(this object obj, string propertyName)
        {
            return Allow(obj, obj.GetType().GetProperty(propertyName).GetSetMethod());
        }

        public static bool AllowGet(this object obj, string propertyName)
        {
            return Allow(obj, obj.GetType().GetProperty(propertyName).GetGetMethod());
        }

        public static bool Allow(this object obj, MethodInfo mi)
        {
            var type = obj.GetType();
            var methodName = mi.Name;
            bool? allowed = null;
            string name = methodName;
            if (methodName.StartsWith("get_"))
            {
                name = methodName.Substring(4);
                allowed = allowed ?? ExecuteAllowMethod(obj, "AllowGet" + name);
            }
            if (methodName.StartsWith("set_"))
            {
                name = methodName.Substring(4);
                allowed = allowed ?? ExecuteAllowMethod(obj, "AllowSet" + name);
            }

            foreach (var rule in Rules)
            {
                allowed = allowed ?? rule(obj, mi);
            }

            allowed = allowed ?? ExecuteAllowMethod(obj, "Allow" + name);

            if (allowed == null) //only use global if no bespoke allow method
            {
                var globalAllowMethod = (type.GetMethod("Allow"));
                if (globalAllowMethod != null)
                {
                    allowed = globalAllowMethod.Invoke(obj, new object[] { mi }) as bool?;

                }
            }
            return allowed ?? true;
        }

        private static bool? ExecuteAllowMethod(object target, string allowMethodName)
        {
            Type type = target.GetType();
            var allowMethod = (type.GetMethod(allowMethodName));
            if (allowMethod != null)
            {
                var allowed = (bool)allowMethod.Invoke(target, null);
                return allowed;
            }
            return null;
        }

        public static IList<Func<dynamic, MethodInfo, bool?>> Rules { get; private set; }
    }
}
