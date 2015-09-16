using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Harden
{
    public interface IAllowRule
    {
        bool? Allow(object o, MethodInfo methodbeingcalled, object context);
    }
}
