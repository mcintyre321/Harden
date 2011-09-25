using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;

namespace Harden
{
    public static class HardenExtensions
    {
        public static bool AllowSet(this object obj, string propertyName)
        {
            return Allow.Set(obj, propertyName);
        }

        public static bool AllowSet(this object obj, PropertyInfo pi)
        {
            return Harden.Allow.Set(obj, pi);
        }
        
        public static bool AllowGet(this object obj, string propertyName)
        {
            return Harden.Allow.Get(obj, propertyName);
          
        }

        public static bool AllowGet(this object obj, PropertyInfo pi)
        {
            return Harden.Allow.Get(obj, pi);
        }


        public static bool AllowCall(this object obj, string methodName)
        {
            return Harden.Allow.Call(obj, methodName);
        }


    }
}
