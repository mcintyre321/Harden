using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Harden
{
    public static class Allow
    {
        public delegate bool? AllowRule(dynamic obj, MethodInfo methodBeingCalled);
        static Allow()
        {
            Rules = new List<AllowRule>();
        }
        public static List<AllowRule> Rules { get; private set; }
        
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
        #region nuts n bolts (DoAllow)

        internal static bool DoAllow(object obj, MethodInfo mi)
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
                var allowed = (bool?)allowMethod.Invoke(target, null);
                return allowed;
            }
            return null;
        }
        #endregion

        public static bool Call(object o, string methodName)
        {
            return Call(o, o.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public));
        }
        public static bool Call(object o, MethodInfo mi)
        {
            return DoAllow(o, mi);
        }
        #region Get
        public static bool Get<T>(Expression<Func<T>> t)
        {
            var tup = StaticReflector.GetObjAndProp(t);
            return DoAllow(tup.Item1, tup.Item2.GetGetMethod());
        }
        public static bool Get(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
            return Get(obj, prop);
        }
        public static bool Get(object o, PropertyInfo pi)
        {
            if (!pi.CanRead) return false;
            var get = pi.GetGetMethod();
            if (get == null) return false;
            return DoAllow(o, get);
        }
        #endregion
        #region set
        public static bool Set(object o, PropertyInfo pi)
        {
            if (!pi.CanWrite) return false;
            var set = pi.GetSetMethod();
            if (set == null) return false;
            return DoAllow(o, set);
        }
        public static bool Set(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
            return Set(obj, prop);
        }
        public static bool Set<T>(Expression<Func<T>> t)
        {
            var tup = StaticReflector.GetObjAndProp(t);
            return DoAllow(tup.Item1, tup.Item2.GetSetMethod());
        }
        #endregion
    }
}