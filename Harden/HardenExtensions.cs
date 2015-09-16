using System.Reflection;

namespace Harden
{
    public static class HardenExtensions
    {
        public static bool AllowSet(this object obj, string propertyName, object context)
        {
            return Allow.Set(obj, propertyName, context);
        }

        public static bool AllowSet(this object obj, PropertyInfo pi, object context)
        {
            return Harden.Allow.Set(obj, pi, context);
        }
        
        public static bool AllowGet(this object obj, string propertyName, object context)
        {
            return Harden.Allow.Get(obj, propertyName, context);
          
        }

        public static bool AllowGet(this object obj, PropertyInfo pi, object context)
        {
            return Harden.Allow.Get(obj, pi, context);
        }


        public static bool AllowCall(this object obj, string methodName, object context)
        {
            return Harden.Allow.Call(obj, methodName, context);
        }


    }
}
