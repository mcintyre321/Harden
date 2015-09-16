using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Harden
{
    public class AllowResult
    {
        private AllowResult()
        {
            
        }
        public static readonly AllowResult Deny = new AllowResult();
        public static readonly AllowResult Defer = new AllowResult();
        public static readonly AllowResult Pass = new AllowResult();

        public static implicit operator AllowResult(bool? value)
        {
            if (value.HasValue)
            {
                return value.Value ? Pass : Deny;
            }
            return Defer;
        }

        public static implicit operator bool?(AllowResult value)
        {
            if (value == null || value == Defer) return null;
            if (value == Pass) return true;
            return false;
        }
    }

public class AllowAttribute
    {
            
    }

    public interface IAllowRule
    {
        AllowResult Allow(object o, MethodInfo methodbeingcalled, object context);
    }
}
