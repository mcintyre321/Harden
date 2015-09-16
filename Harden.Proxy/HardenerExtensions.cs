using System.Reflection;

namespace Harden
{
    public static class HardenerExtensions
    {
        public static bool IsProperty(this MethodInfo mi)
        {
            return mi.Name.StartsWith("get_") || mi.Name.StartsWith("set_");
        }
        public static T Harden<T>(this T t) where T : class
        {
            return Hardener.Harden(t);
        }
    }
}