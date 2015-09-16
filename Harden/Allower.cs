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

        public bool DoAllow(object obj, MethodInfo mi, object context)
        {
            return Rules.Select(r => r(obj, mi, context)).FirstOrDefault(r => r != null) ?? true;
        }
        public bool Call(object o, string methodName, object context)
        {
            return Call(o, o.GetType().GetRuntimeMethod(methodName), context);
        }
        public bool Call(object o, MethodInfo mi, object context)
        {
            return DoAllow(o, mi, context);
        }
        #region Get
        public bool Get<T>(Expression<Func<T>> t, object context)
        {
            var tup = StaticReflector.GetObjAndProp(t);
            return DoAllow(tup.Item1, tup.Item2.GetMethod, context);
        }
        public bool Get(object obj, string propertyName, object context)
        {
            var prop = obj.GetType().GetRuntimeProperty(propertyName);
            return Get(obj, prop, context);
        }
        public bool Get(object o, PropertyInfo pi, object context)
        {
            if (!pi.CanRead) return false;
            var get = pi.GetMethod;
            if (get == null) return false;
            return DoAllow(o, get, context);
        }
        #endregion
        #region set
        public bool Set(object o, PropertyInfo pi, object context)
        {
            if (!pi.CanWrite) return false;
            var set = pi.SetMethod;
            if (set == null) return false;
            return DoAllow(o, set, context);
        }
        public bool Set(object obj, string propertyName, object context)
        {
            var prop = obj.GetType().GetRuntimeProperty(propertyName);
            return Set(obj, prop, context);
        }
        public bool Set<T>(Expression<Func<T>> t, object context)
        {
            var tup = StaticReflector.GetObjAndProp(t);
            return DoAllow(tup.Item1, tup.Item2.SetMethod, context);
        }
        #endregion

       
    }
}