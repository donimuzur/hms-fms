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
    public class VehicleSpectService : IVehicleSpectService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_VEHICLE_SPECT> _vendorRepository;

        public VehicleSpectService(IUnitOfWork uow)
        {
            _uow = uow;
            _vendorRepository = _uow.GetGenericRepository<MST_VEHICLE_SPECT>();
        }

        public List<MST_VEHICLE_SPECT> GetVehicleSpect()
        {
            return _vendorRepository.Get().ToList();
        }

        public MST_VEHICLE_SPECT GetVehicleSpectById(int MstVehicleSpectId)
        {
            return _vendorRepository.GetByID(MstVehicleSpectId);
        }

        public void save(MST_VEHICLE_SPECT dbVehicleSpect)
        {
            _uow.GetGenericRepository<MST_VEHICLE_SPECT>().InsertOrUpdate(dbVehicleSpect);
            _uow.SaveChanges();
        }
    }
}
