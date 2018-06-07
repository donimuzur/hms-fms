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
    public class ArchSalesVolumeService : IArchSalesVolumeService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_SALES_VOLUME> _archSalesVolumeRepository;
        public ArchSalesVolumeService(IUnitOfWork uow)
        {
            _uow = uow;
            _archSalesVolumeRepository = uow.GetGenericRepository<ARCH_MST_SALES_VOLUME>();
        }
        public void Save(ARCH_MST_SALES_VOLUME db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_SALES_VOLUME>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
