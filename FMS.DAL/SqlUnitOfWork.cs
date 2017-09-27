using System.Data.Entity;
using FMS.BusinessObject;
using FMS.Contract;
using NLog;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FMS.DAL
{
    public class SqlUnitOfWork : IDisposable, IUnitOfWork
    {
        private ILogger _logger;

        public SqlUnitOfWork(ILogger logger)
        {
            _logger = logger;
        }

        //load a context automatically
        // the context is disposed when this UOW obejct is disposed > at end of webrequest (simple injector weblifestyle request)
        private FMSEntities _context = new FMSEntities();

        public IGenericRepository<T> GetGenericRepository<T>()
            where T : class
        {
            //Use a dictionary for this if really needed ?
            //if (this._VEHICUTRepository == null)
            //    this._VEHICUTRepository = new GenericRepository<VEHICUT>(context);

            return new SqlGenericRepository<T>(_context, _logger); ;
        }

        /// <summary>
        /// Saves context changes
        /// </summary>
        public void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                LogEntityValidationErrors(e.EntityValidationErrors);
                throw;
            }
        }


        /// <summary>
        /// Saves context changes
        /// </summary>
        public Task SaveAsync()
        {
            try
            {
                return _context.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                LogEntityValidationErrors(e.EntityValidationErrors);
                throw;
            }
        }

        /// <summary>
        /// Reverts the changes.
        /// </summary>
        public void RevertChanges()
        {
            //overwrite the existing context with a new, fresh one to revert all the changes
            _context = new FMSEntities();
        }

       


        /// <summary>
        /// Logs the entity validation errors.
        /// </summary>
        /// <param name="entityValidationErrors">The entity validation errors.</param>
        private void LogEntityValidationErrors(IEnumerable<DbEntityValidationResult> entityValidationErrors)
        {
            foreach (var entityValidationError in entityValidationErrors)
            {
                _logger.Error(string.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                    entityValidationError.Entry.Entity.GetType().Name, entityValidationError.Entry.State));
                foreach (var validationErrors in entityValidationError.ValidationErrors)
                {
                    _logger.Error(string.Format("- Property: \"{0}\", Error: \"{1}\"",
                        validationErrors.PropertyName, validationErrors.ErrorMessage));
                }
            }
        }

        private bool disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// the dispose method is called automatically by the injector depending on the lifestyle
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}
