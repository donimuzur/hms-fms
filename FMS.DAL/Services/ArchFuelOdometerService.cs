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
    public class ArchFuelOdometerService : IArchFuelOdometerService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_FUEL_ODOMETER> _archFuelOdometerRepository;

        public ArchFuelOdometerService(IUnitOfWork   uow)
        {
            _uow = uow;
            _archFuelOdometerRepository = uow.GetGenericRepository<ARCH_MST_FUEL_ODOMETER>();
        }
        public void Save(ARCH_MST_FUEL_ODOMETER db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_FUEL_ODOMETER>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
