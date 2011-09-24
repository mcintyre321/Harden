using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Castle.DynamicProxy;

namespace Harden
{
    public class Hardener
    {
        static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static T Harden<T>(T pansy) where T : class
        {
            return proxyGenerator.CreateClassProxyWithTarget<T>(pansy, new HardenInterceptor());
        }

        public static T Create<T>() where T : class
        {
            return proxyGenerator.CreateClassProxy<T>(new HardenInterceptor());
        }

        public static T Create<T>(Expression<Func<T>> create) where T : class
        {
            var constructorArguments = new List<object>();
            foreach (var arg in ((NewExpression)create.Body).Arguments)
            {
                if (arg is ConstantExpression)
                {
                    constructorArguments.Add(((ConstantExpression)arg).Value);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            return (T)proxyGenerator.CreateClassProxy(typeof(T), constructorArguments.ToArray(), new HardenInterceptor());
        }


        public static object Create(Type t)
        {
            return proxyGenerator.CreateClassProxy(t, new HardenInterceptor());
        }
    }

    public static class HardenerExtensions
    {
        public static bool IsProperty(this MethodInfo mi)
        {
            return mi.Name.StartsWith("get_") || mi.Name.StartsWith("set_");
        }
        public static T Harden<T>(this T t) where T : class
        {
            return Hardener.Harden(t);
        }
    }
}
