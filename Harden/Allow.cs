using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Castle.Core.Internal;

namespace Harden
{
    public class Allow
    {
        static Allow()
        {
            Allower = new Allower();
        }

        public static Allower Allower;

        public delegate bool? AllowRule(dynamic obj, MethodInfo methodBeingCalled, object context);

        public static bool? CheckAttributes(object obj, MethodInfo methodbeingcalled, object context)
        {
            var attributes = methodbeingcalled.GetAttributes<IAllowRule>();
            var propertyInfo = GetPropFromMethod(methodbeingcalled.DeclaringType, methodbeingcalled);
            if (propertyInfo != null) attributes = attributes.Concat(propertyInfo.GetAttributes<IAllowRule>()).ToArray();
            return attributes
                .Select(r => r.Allow(obj, methodbeingcalled)).FirstOrDefault(v => v != null);
        }

        public static PropertyInfo GetPropFromMethod(Type t, MethodInfo method)
        {
            if (!method.IsSpecialName) return null;
            return t.GetProperty(method.Name.Substring(4),
              BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
        }

      


        public static AllowRule CheckAllowGetX = (o, mi, c) =>
        {
            if (mi.Name.StartsWith("get_"))
            {
                var name = mi.Name.Substring(4);
                return ExecuteAllowMethod(o, "AllowGet" + name, c);
            }
            return null;
        };
        public static AllowRule CheckAllowSetX = (o, mi, c) =>
        {
            if (mi.Name.StartsWith("set_"))
            {
                var name = mi.Name.Substring(4);
                return ExecuteAllowMethod(o, "AllowSet" + name, c);
            }
            return null;
        };

        public static AllowRule CheckAllowX = (o, mi, c) =>
        {
            string name = mi.Name;
            if (name.StartsWith("get_") || name.StartsWith("set_"))
            {
                name = mi.Name.Substring(4);
            }
            return ExecuteAllowMethod(o, "Allow" + name, c);
        };

        public static AllowRule CheckClassLevelAllow = (o, mi, c) =>
        {
            var globalAllowMethod = (mi.DeclaringType.GetMethod("Allow"));
            if (globalAllowMethod != null)
            {
                return globalAllowMethod.Invoke(o, new object[] {mi}) as bool?;
            }
            return null;
        };
        #region nuts n bolts (DoAllow)

         

        private static bool? ExecuteAllowMethod(object target, string allowMethodName, object context)
        {
            Type type = target.GetType();
            var allowMethod = (type.GetMethod(allowMethodName));
            if (allowMethod != null)
            {
                var parameters = allowMethod.GetParameters();
                var args = parameters.Length == 0
                    ? new object[] {}
                    : new object[] {context};
                var allowed = (bool?)allowMethod.Invoke(target, args);
                return allowed;
            }
            return null;
        }
        #endregion

        public static bool Call(object o, string methodName, object context)
        {
            return Call(o, o.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public), context);
        }
        public static bool Call(object o, MethodInfo mi, object context)
        {
            return Allower.Call(o, mi, context);
        }

        #region Get
        public static bool Get<T>(Expression<Func<T>> t, object context)
        {
            return Allower.Get(t, context);
        }

        public static bool Get(object obj, string propertyName, object context)
        {
            return Allower.Get(obj, propertyName, context);
        }

        public static bool Get(object o, PropertyInfo pi, object context)
        {
            return Allower.Get(o, pi, context);
        }
        #endregion
        #region set
        public static bool Set(object o, PropertyInfo pi, object context)
        {
            return Allower.Set(o, pi, context);
        }
        public static bool Set(object obj, string propertyName, object context)
        {
            return Allower.Set(obj, propertyName, context);
        }
        public static bool Set<T>(Expression<Func<T>> t, object context)
        {
            return Allower.Set(t, context);
        }
        #endregion
    }
}