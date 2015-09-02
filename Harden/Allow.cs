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

        public delegate bool? AllowRule(dynamic obj, MethodInfo methodBeingCalled);

        public static bool? CheckAttributes(object obj, MethodInfo methodbeingcalled)
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
            return Allower.Call(o, mi);
        }

        #region Get
        public static bool Get<T>(Expression<Func<T>> t)
        {
            return Allower.Get(t);
        }

        public static bool Get(object obj, string propertyName)
        {
            return Allower.Get(obj, propertyName);
        }

        public static bool Get(object o, PropertyInfo pi)
        {
            return Allower.Get(o, pi);
        }
        #endregion
        #region set
        public static bool Set(object o, PropertyInfo pi)
        {
            return Allower.Set(o, pi);
        }
        public static bool Set(object obj, string propertyName)
        {
            return Allower.Set(obj, propertyName);
        }
        public static bool Set<T>(Expression<Func<T>> t)
        {
            return Allower.Set(t);
        }
        #endregion
    }
}