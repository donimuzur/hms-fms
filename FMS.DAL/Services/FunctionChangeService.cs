using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class FunctionChangeService : IFunctionChangeService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<FUNCTION_CHANGE> _functionChangeRepo;

        public  FunctionChangeService(IUnitOfWork uow)
        {
            _uow = uow;
            _functionChangeRepo = uow.GetGenericRepository<FUNCTION_CHANGE>();
        }

        public List<FUNCTION_CHANGE> GetListFunctionChange()
        {
            return _functionChangeRepo.Get().Where(x => x.DATE_SEND == null).ToList();
        }

        public void Save(FUNCTION_CHANGE DbFunctionChange)
        {
            _functionChangeRepo.InsertOrUpdate(DbFunctionChange);
            _uow.SaveChanges();
        }
    }
}
