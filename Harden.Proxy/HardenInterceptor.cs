using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace Harden
{
    public class HardenInterceptor : IInterceptor
    {
        private Allower _allower;

        public HardenInterceptor(Allower allower)
        {
            _allower = allower;
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.StartsWith("Allow") || invocation.Method.Name.StartsWith("Validate"))
            {
                invocation.Proceed();
                return;
            }
            if (_allower.DoAllow(invocation.Proxy, invocation.Method, null) == false)
            {
                throw new HardenException("Not allowed to call " + invocation.Method.Name);
            }

            Validation.Execute(invocation.Proxy, invocation.Method, invocation.Arguments);

            invocation.Proceed();
        }

    }
}