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
    public class ArchGsService : IArchGsService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_GS> _archGsRepository;

        public ArchGsService(IUnitOfWork uow)
        {
            _uow = uow;
            _archGsRepository = uow.GetGenericRepository<ARCH_MST_GS>();
        }
        public void Save(ARCH_MST_GS db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_GS>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
