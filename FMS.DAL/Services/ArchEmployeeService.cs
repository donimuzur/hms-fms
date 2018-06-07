using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class ArchEmployeeService : IArchEmployeeService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_EMPLOYEE> _archEmployeeRepository;
        public ArchEmployeeService(IUnitOfWork uow)
        {
            _uow = uow;
            _archEmployeeRepository = uow.GetGenericRepository<ARCH_MST_EMPLOYEE>();
        }
        public void Save(ARCH_MST_EMPLOYEE db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_EMPLOYEE>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
