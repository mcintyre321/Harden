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
            if (invocation.Method.Name.StartsWith("Allow") || invocation.Method.Name.StartsWith("Validate"))
            {
                invocation.Proceed();
                return;
            }

            var type = invocation.InvocationTarget.GetType();
            
            if ((invocation.InvocationTarget.Allow(invocation.Method)) == false)
            {
                throw new HardenException("Not allowed to call " + invocation.Method.Name);
            }

            if (invocation.Arguments.Length > 0) // we may need to validate these arguments...
            {
                string name = invocation.Method.Name;
                if (name.StartsWith("set_"))
                {
                    name = name.Substring(4);
                }
                var validateMethod = type.GetMethod("Validate" + name);
                if (validateMethod != null)
                {
                    object[] args = null;

                    if (invocation.Arguments.Length > 1)
                    {
                        var argsQ = from p in validateMethod.GetParameters()
                                    from a in invocation.Arguments.Zip(invocation.Method.GetParameters(), (arg, pi) => new { pi.Name, arg })
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
                            throw new ValidationException(invocation.Proxy, validationErrors);

                        }

                    }
                }
            }


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