using System;
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
        public static object Create(Type t)
        {
            return proxyGenerator.CreateClassProxy(t, new HardenInterceptor());
        }
    }

    public static class HardenerExtensions
    {
        public static T Harden<T>(this T t) where T : class
        {
            return Hardener.Harden(t);
        }
    }
}
