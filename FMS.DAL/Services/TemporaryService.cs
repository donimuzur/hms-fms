using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class TemporaryService : ITemporaryService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<TRA_TEMPORARY> _traTempRepository;

        public TemporaryService(IUnitOfWork uow)
        {
            _uow = uow;
            _traTempRepository = _uow.GetGenericRepository<TRA_TEMPORARY>();
        }

        public TRA_TEMPORARY GetTemporaryById(long id)
        {
            Expression<Func<TRA_TEMPORARY, bool>> queryFilter = PredicateHelper.True<TRA_TEMPORARY>();

            queryFilter = queryFilter.And(c => c.TRA_TEMPORARY_ID == id);

            return _traTempRepository.Get(queryFilter).FirstOrDefault();
        }

        public void saveTemporary(TRA_TEMPORARY dbTraTemporary, Login userlogin)
        {
            _uow.GetGenericRepository<TRA_TEMPORARY>().InsertOrUpdate(dbTraTemporary, userlogin, Enums.MenuList.TraTmp);
            _uow.SaveChanges();
        }
    }
}
