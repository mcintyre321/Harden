using System;
using System.Reflection;
using Castle.DynamicProxy;

namespace Harden
{
    public class HardenInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.StartsWith("Allow"))
            {
                invocation.Proceed();
                return;
            }

            var type = invocation.InvocationTarget.GetType();
            string name = invocation.Method.Name;
            MethodInfo allowMethod = null;
            if (invocation.Method.Name.StartsWith("get_"))
            {
                name = invocation.Method.Name.Substring(4);
                allowMethod = allowMethod ?? AssertAllowed(invocation, "AllowGet" + name, type);
            }
            if (invocation.Method.Name.StartsWith("set_"))
            {
                name = invocation.Method.Name.Substring(4);
                allowMethod = allowMethod ?? AssertAllowed(invocation, "AllowSet" + name, type);
            }

            allowMethod = allowMethod ?? AssertAllowed(invocation, "Allow" + name, type);

            if (allowMethod == null) //only use global if no bespoke allow method
            {
                var globalAllowMethod = (type.GetMethod("Allow"));
                if (globalAllowMethod != null)
                {
                    var allowed = (bool) globalAllowMethod.Invoke(invocation.InvocationTarget, new object[] {invocation.Method});
                    if (!allowed)
                    {
                        throw new HardenException("Not allowed to call " + invocation.Method.Name);
                    }
                }
            }
            invocation.Proceed();
        }

        private MethodInfo AssertAllowed(IInvocation invocation, string allowMethodName, Type type)
        {
            var allowMethod = (type.GetMethod(allowMethodName));
            if (allowMethod != null)
            {
                var allowed = (bool) allowMethod.Invoke(invocation.InvocationTarget, null);
                if (!allowed)
                {
                    throw new HardenException("Not allowed to call " + invocation.Method.Name);
                }
            }
            return allowMethod;
        }
    }
}