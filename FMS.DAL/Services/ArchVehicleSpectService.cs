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
    public class ArchVehicleSpectService : IArchVehicleSpectService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_VEHICLE_SPECT> _archVehicleSpectRepository;
        public ArchVehicleSpectService(IUnitOfWork uow)
        {
            _uow = uow;
            _archVehicleSpectRepository = uow.GetGenericRepository<ARCH_MST_VEHICLE_SPECT>();
        }
        public void Save(ARCH_MST_VEHICLE_SPECT db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_VEHICLE_SPECT>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
    }
}
