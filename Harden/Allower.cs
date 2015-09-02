using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Harden
{
    public class Allower
    {
        public List<Allow.AllowRule> Rules { get; } = new List<Allow.AllowRule>()
        {
            Allow.CheckAllowGetX,
            Allow.CheckAllowSetX,
            Allow.CheckAllowX,
            Allow.CheckClassLevelAllow,
            Allow.CheckAttributes
        };

        internal bool DoAllow(object obj, MethodInfo mi)
        {
            return Rules.Select(r => r(obj, mi)).FirstOrDefault(r => r != null) ?? true;
        }
        public bool Call(object o, string methodName)
        {
            return Call(o, o.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public));
        }
        public bool Call(object o, MethodInfo mi)
        {
            return DoAllow(o, mi);
        }
        #region Get
        public bool Get<T>(Expression<Func<T>> t)
        {
            var tup = StaticReflector.GetObjAndProp(t);
            return DoAllow(tup.Item1, tup.Item2.GetGetMethod());
        }
        public bool Get(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
            return Get(obj, prop);
        }
        public bool Get(object o, PropertyInfo pi)
        {
            if (!pi.CanRead) return false;
            var get = pi.GetGetMethod();
            if (get == null) return false;
            return DoAllow(o, get);
        }
        #endregion
        #region set
        public bool Set(object o, PropertyInfo pi)
        {
            if (!pi.CanWrite) return false;
            var set = pi.GetSetMethod();
            if (set == null) return false;
            return DoAllow(o, set);
        }
        public bool Set(object obj, string propertyName)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public);
            return Set(obj, prop);
        }
        public bool Set<T>(Expression<Func<T>> t)
        {
            var tup = StaticReflector.GetObjAndProp(t);
            return DoAllow(tup.Item1, tup.Item2.GetSetMethod());
        }
        #endregion

       
    }
}