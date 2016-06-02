using System;
using System.Linq.Expressions;
using System.Reflection;

namespace SEV.Common
{
    public static class LambdaExpressionHelper
    {
        public static bool IsCollectionExpression(LambdaExpression expression)
        {
            string exprBodyTypeName = expression.Body.Type.Name;

            return exprBodyTypeName.StartsWith("IList") ||
                   exprBodyTypeName.StartsWith("List") ||
                   exprBodyTypeName.StartsWith("ICollection") ||
                   exprBodyTypeName.StartsWith("IEnumerable");
        }

        public static string GetPropertyName(LambdaExpression expression)
        {
            if (!(expression.Body is MemberExpression))
            {
                throw new ArgumentException(String.Format(@"'{0}' is not MemberExpression", expression), "expression");
            }
            return ((MemberExpression)expression.Body).Member.Name;
        }

        public static PropertyInfo GetExpressionMethod(LambdaExpression expression)
        {
            if (!(expression.Body is MemberExpression))
            {
                throw new ArgumentException(String.Format(@"'{0}' is not MemberExpression", expression), "expression");
            }
            return (PropertyInfo)((MemberExpression)expression.Body).Member;
        }
    }
}