using System;
using System.Collections.Generic;

namespace Harden
{
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