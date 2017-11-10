using System;
using System.Linq.Expressions;

namespace FMS.Utils
{
    public class OrderByHelper
    {
        public static Expression<Func<T, object>> GetOrderByFunction<T>(string sord) where T : class
        {
            var param = Expression.Parameter(typeof(T), "p");
            var parts = sord.Split('.');

            Expression parent = param;

            foreach (var part in parts)
            {
                parent = Expression.Property(parent, part);
            }

            var sortExpression = Expression.Lambda<Func<T, object>>(parent, param);
            return sortExpression;
        }
    }
}
