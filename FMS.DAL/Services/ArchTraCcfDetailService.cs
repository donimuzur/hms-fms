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
    public class ArchTraCcfDetailService : IArchTraCcfDetailService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_TRA_CCF_DETAIL> _archTraCcfDetailRepository;
        public ArchTraCcfDetailService(IUnitOfWork uow)
        {
            _uow = uow;
            _archTraCcfDetailRepository = _uow.GetGenericRepository<ARCH_TRA_CCF_DETAIL>();
        }
        public void Save(ARCH_TRA_CCF_DETAIL db,Login Login)
        {
            _uow.GetGenericRepository<ARCH_TRA_CCF_DETAIL>().InsertOrUpdate(db, Login, Enums.MenuList.MasterData);
        }

    }
}
