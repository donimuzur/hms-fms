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
    public class ArchTraCafProgressService : IArchTraCafProgressService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_CAF_PROGRESS> _archTraCfProgress;
        public ArchTraCafProgressService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCfProgress = _uow.GetGenericRepository<ARCH_TRA_CAF_PROGRESS>();
        }

        public void Save(ARCH_TRA_CAF_PROGRESS db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_TRA_CAF_PROGRESS>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
