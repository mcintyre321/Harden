using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Harden
{
    class StaticReflector
    {
        public static Tuple<object, PropertyInfo> GetObjAndProp<T>(Expression<Func<T>> t)
        {
            var propertyGetExpression = t.Body as MemberExpression;

            // "s" is replaced by a field access on a compiler-generated class from the closure
            var fieldOnClosureExpression = propertyGetExpression.Expression as MemberExpression;

            // Find the compiler-generated class
            var closureClassExpression = fieldOnClosureExpression.Expression as ConstantExpression;
            var closureClassInstance = closureClassExpression.Value;

            // Find the field value, in this case it's a reference to the "s" variable
            var closureFieldInfo = fieldOnClosureExpression.Member as FieldInfo;
            var closureFieldValue = closureFieldInfo.GetValue(closureClassInstance);

            // We know that the Expression is a property access so we get the PropertyInfo instance
            // And even access the value (yes compiling the expression would have been simpler :D)
            var propertyInfo = propertyGetExpression.Member as PropertyInfo;
            //var propertyValue = propertyInfo.GetValue(closureFieldValue, null);

            return Tuple.Create(closureFieldValue, propertyInfo);
        }
        public static Tuple<object, MethodInfo> GetObjAndMethod<T>(Expression<Func<T>> t)
        {
            var propertyGetExpression = t.Body as MemberExpression;

            // "s" is replaced by a field access on a compiler-generated class from the closure
            var fieldOnClosureExpression = propertyGetExpression.Expression as MemberExpression;

            // Find the compiler-generated class
            var closureClassExpression = fieldOnClosureExpression.Expression as ConstantExpression;
            var closureClassInstance = closureClassExpression.Value;

            // Find the field value, in this case it's a reference to the "s" variable
            var closureFieldInfo = fieldOnClosureExpression.Member as FieldInfo;
            var closureFieldValue = closureFieldInfo.GetValue(closureClassInstance);

            // We know that the Expression is a property access so we get the PropertyInfo instance
            // And even access the value (yes compiling the expression would have been simpler :D)
            var propertyInfo = propertyGetExpression.Member as MethodInfo;
            //var propertyValue = propertyInfo.GetValue(closureFieldValue, null);

            return Tuple.Create(closureFieldValue, propertyInfo);
        }
    }
}