using Castle.DynamicProxy;

namespace Harden
{
    public class Hardener
    {
        static readonly ProxyGenerator proxyGenerator = new ProxyGenerator();

        public static T Harden<T>(T pansy) where T : class
        {
            return proxyGenerator.CreateClassProxy<T>(new HardenInterceptor());
        }
    }
}
