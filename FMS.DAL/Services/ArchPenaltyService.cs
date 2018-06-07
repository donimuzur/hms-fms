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
    public class ArchPenaltyService : IArchPenaltyService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_PENALTY> _archPenaltyRepository;

        public ArchPenaltyService(IUnitOfWork uow)
        {
            _uow = uow;
            _archPenaltyRepository = uow.GetGenericRepository<ARCH_MST_PENALTY>();
        }
        public void Save(ARCH_MST_PENALTY db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_PENALTY>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
