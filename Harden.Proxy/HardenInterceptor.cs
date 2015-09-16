using System;
using System.Collections.Generic;
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
    public class ValidationException : Exception
    {
        private List<Error> _errors = new List<Error>();
        private object _object;

        public ValidationException(object o, IEnumerable<Error> errors)
        {
            _object = o;
            _errors.AddRange(errors);
        }

        public IEnumerable<Error> Errors
        {
            get { return _errors; }
        }

        public object Object
        {
            get { return _object; }
        }
    }
}