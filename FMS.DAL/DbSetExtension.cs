using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL
{
    public static class DbSetExtension
    {
        public static void SafeAttach<T>(
           this DbContext context,
           T entity,
           Func<T, object> keyFn) where T : class
        {
            var existing = context.Set<T>().Local
                .FirstOrDefault(x => Equals(keyFn(x), keyFn(entity)));
            if (existing != null)
                context.Entry(existing).State = EntityState.Detached;

            context.Set<T>().Attach(entity);
        }
    }
}
