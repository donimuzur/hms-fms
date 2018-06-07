using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class ArchFunctionGroupService : IArchFunctionGroupService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_FUNCTION_GROUP> _archFunctionGroupRepository;
        public ArchFunctionGroupService(IUnitOfWork uow)
        {
            _uow = uow;
            _archFunctionGroupRepository = uow.GetGenericRepository<ARCH_MST_FUNCTION_GROUP>();
        }
        public void Save(ARCH_MST_FUNCTION_GROUP db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_FUNCTION_GROUP>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
