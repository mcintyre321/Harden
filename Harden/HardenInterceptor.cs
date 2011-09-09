using System;
using System.Collections.Generic;
using System.Linq;
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
                    var allowed = (bool)globalAllowMethod.Invoke(invocation.InvocationTarget, new object[] { invocation.Method });
                    if (!allowed)
                    {
                        throw new HardenException("Not allowed to call " + invocation.Method.Name);
                    }
                }
            }

            if (invocation.Arguments.Length > 0) // we may need to validate these arguments...
            {
                var validateMethod = type.GetMethod("Validate" + name);
                if (validateMethod != null)
                {
                    object[] args = null;

                    if (invocation.Arguments.Length > 1)
                    {
                        var argsQ = from p in validateMethod.GetParameters()
                                    from a in
                                        invocation.Arguments.Zip(invocation.Method.GetParameters(),
                                                                 (arg, pi) => new { pi.Name, arg })
                                    where p.Name == a.Name
                                    select a.arg;
                        args = argsQ.ToArray();
                    }
                    else
                    {
                        args = invocation.Arguments;
                    }
                    var validationErrors = validateMethod.Invoke(invocation.InvocationTarget, args) as IEnumerable<Error>;
                    if (validationErrors != null)
                    {
                        validationErrors = validationErrors.ToArray();
                        if (validationErrors.Any())
                        {
                            if (invocation.Method.Name.StartsWith("set_"))
                            {
                                foreach (var validationError in validationErrors)
                                {
                                    validationError.Field = name;
                                }
                            }
                            throw new ValidationException(invocation.InvocationTarget, validationErrors);

                        }

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
                var allowed = (bool)allowMethod.Invoke(invocation.InvocationTarget, null);
                if (!allowed)
                {
                    throw new HardenException("Not allowed to call " + invocation.Method.Name);
                }
            }
            return allowMethod;
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

    public class Error
    {
        public Error(string field, string message)
        {
            Field = field;
            Message = message;
        }

        public Error(string message)
        {
            Message = message;
        }


        public string Message { get; private set; }

        public string Field { get; internal set; }
    }
}