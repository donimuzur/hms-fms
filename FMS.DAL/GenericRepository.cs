using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using DFIS.Contracts;
using DFIS.EntitiesDAL.EDMX;

namespace DFIS.EntitiesDAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {

        internal DFISContextDB Context;
        internal DbSet<TEntity> DbSet;
        public GenericRepository(DFISContextDB contextEntities)
        {
            Context = contextEntities;
            DbSet = Context.Set<TEntity>();
        }

        public int Save()
        {
            try
            {
                Context.SaveChanges();
            }
            catch (Exception e)
            {
                var message = e;
            }
            return 0;
        }

        public void Save(string controller, string userid)
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            TransactionLog transactionLog = new TransactionLog();
           
            var entries = Context.ChangeTracker.Entries();
            foreach(var entry in entries){
                string state = (entry.State.ToString() == "Added")?"Insert":"Update";
                string name = entry.Entity.GetType().FullName.ToString();
                string remark = "";
                int i = 0;

                string tableName = entry.Entity.GetType().Name;            
                              

                transactionLog.IDFunction = Context.MasterFunctions.Where(x => x.FunctionName.Equals(controller)).Select(x => x.IDFunction).SingleOrDefault();
                transactionLog.IDUser = userid;
                transactionLog.Action = state;
                transactionLog.Remarks = remark;
                transactionLog.CreatedDate = DateTime.Now;
            }

            if (Context.SaveChanges() > 0)
            {
                Context.TransactionLogs.Add(transactionLog);
                Context.SaveChanges();
            }
            
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }
        
        public void Update(TEntity entityToUpdate)
        {
            //DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void UpdateNew(TEntity entityToUpdate)
        {

           
            //DbSet.Attach(entityToUpdate);
            Context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public void InsertOrUpdate(TEntity entity)
        {
            if (!Exists(entity))
                Insert(entity);
            else
                Update(entity);
        }

        public bool Exists(TEntity entity)
        {
            var objContext = ((IObjectContextAdapter)Context).ObjectContext;
            var objSet = objContext.CreateObjectSet<TEntity>();
            var entityKey = objContext.CreateEntityKey(objSet.EntitySet.Name, entity);

            Object foundEntity;
            var exists = objContext.TryGetObjectByKey(entityKey, out foundEntity);
            return (exists);
        }

        public void DeleteAll()
        {
            DbSet.RemoveRange(DbSet);
        }

        public void Delete(object id)
        {
            var entityToDelete = DbSet.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public TEntity GetByID(object id)
        {
            return DbSet.Find(id);
        }

        public TEntity GetByName(object name)
        {
            return DbSet.Find(name);
        }

        public TEntity GetByID(params object[] keyValues)
        {
            return DbSet.Find(keyValues);
        }
        public IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            query = includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            return orderBy != null ? orderBy(query).ToList() : query.ToList();
        }

        public IEnumerable<TEntity> Get(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter); //.Skip(pageIndex).Take(pageSize);
            }

            query = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

            var res = orderBy != null ? orderBy(query).ToList() : query.ToList();

            return res.Skip(pageIndex).Take(pageSize);
        }


    }
}
