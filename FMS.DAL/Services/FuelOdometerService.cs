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
    public class FuelOdometerService : IFuelOdometerService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<MST_FUEL_ODOMETER> _FuelOdometerRepository;
        public FuelOdometerService(IUnitOfWork uow)
        {
            _uow = uow;
            _FuelOdometerRepository = _uow.GetGenericRepository<MST_FUEL_ODOMETER>();
        }

        public List<MST_FUEL_ODOMETER> GetFuelOdometer()
        {
            return _FuelOdometerRepository.Get().ToList();
        }
    }
}
