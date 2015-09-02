using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Castle.DynamicProxy;

namespace Harden
{
    public class Hardener
    {
        static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static T Harden<T>(T pansy) where T : class
        {
            return proxyGenerator.CreateClassProxyWithTarget<T>(pansy, new HardenInterceptor(Allow.Allower));
        }

        public static T Create<T>(Allower allower) where T : class
        {
            return proxyGenerator.CreateClassProxy<T>(new HardenInterceptor(allower));
        }

        public static T Create<T>(Expression<Func<T>> create, Allower allower) where T : class
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
            return (T)proxyGenerator.CreateClassProxy(typeof(T), constructorArguments.ToArray(), new HardenInterceptor(allower));
        }


        public static object Create(Type t, Allower allower)
        {
            return proxyGenerator.CreateClassProxy(t, new HardenInterceptor(allower));
        }
    }
}
