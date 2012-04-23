using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Harden
{
    public static class Allow
    {
        public delegate bool? AllowRule(dynamic obj, MethodInfo methodBeingCalled);

        static Allow()
        {
            Rules = new List<AllowRule>()
            {
                CheckAllowGetX,
                CheckAllowSetX,
                CheckAllowX,
                CheckClassLevelAllow
            };
        }

        public static List<AllowRule> Rules { get; private set; }


        public static AllowRule CheckAllowGetX = (o, mi) =>
        {
            if (mi.Name.StartsWith("get_"))
            {
                var name = mi.Name.Substring(4);
                return ExecuteAllowMethod(o, "AllowGet" + name);
            }
            return null;
        };
        public static AllowRule CheckAllowSetX = (o, mi) =>
        {
            if (mi.Name.StartsWith("set_"))
            {
                var name = mi.Name.Substring(4);
                return ExecuteAllowMethod(o, "AllowSet" + name);
            }
            return null;
        };

        public static AllowRule CheckAllowX = (o, mi) =>
        {
            string name = mi.Name;
            if (name.StartsWith("get_") || name.StartsWith("set_"))
            {
                name = mi.Name.Substring(4);
            }
            return ExecuteAllowMethod(o, "Allow" + name);
        };

        public static AllowRule CheckClassLevelAllow = (o, mi) =>
        {
            var globalAllowMethod = (mi.DeclaringType.GetMethod("Allow"));
            if (globalAllowMethod != null)
            {
                return globalAllowMethod.Invoke(o, new object[] {mi}) as bool?;
            }
            return null;
        };
        #region nuts n bolts (DoAllow)

        internal static bool DoAllow(object obj, MethodInfo mi)
        {
            return Rules.Select(r => r(obj, mi)).FirstOrDefault(r => r != null) ?? true;
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