using System.Linq.Expressions;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Utils;

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
            var vendor = _vendorRepository.GetByID(MstVehicleSpectId);
            return vendor;
        }

        public void save(MST_VEHICLE_SPECT dbVehicleSpect)
        {
            //_uow.GetGenericRepository<MST_VEHICLE_SPECT>().InsertOrUpdate(dbVehicleSpect);
            //_uow.SaveChanges();
            _vendorRepository.InsertOrUpdate(dbVehicleSpect);
            _uow.SaveChanges();
        }

        public void save(MST_VEHICLE_SPECT dbVehicleSpect, Login userLogin)
        {
            //_uow.GetGenericRepository<MST_VEHICLE_SPECT>().InsertOrUpdate(dbVehicleSpect);
            //_uow.SaveChanges();
            _vendorRepository.InsertOrUpdate(dbVehicleSpect, userLogin, Enums.MenuList.MasterVehicleSpect);
            _uow.SaveChanges();
        }

        public List<MST_VEHICLE_SPECT> GetExistingVehicleSpectByParam(VehicleSpectDto dto)
        {
            Expression<Func<MST_VEHICLE_SPECT, bool>> queryFilterCrf = c => c.IS_ACTIVE;


            queryFilterCrf = queryFilterCrf.And(x => x.MANUFACTURER == dto.Manufacturer);
            queryFilterCrf = queryFilterCrf.And(x => x.MODEL == dto.Models);
            queryFilterCrf = queryFilterCrf.And(x => x.SERIES == dto.Series);
            queryFilterCrf = queryFilterCrf.And(x => x.TRANSMISSION == dto.Transmission);
            queryFilterCrf = queryFilterCrf.And(x => x.YEAR == dto.Year);
            queryFilterCrf = queryFilterCrf.And(x => x.BODY_TYPE == dto.BodyType);
            queryFilterCrf = queryFilterCrf.And(x => x.COLOUR == dto.Color);
            queryFilterCrf = queryFilterCrf.And(x => x.FUEL_TYPE == dto.FuelTypeSpect);
            queryFilterCrf = queryFilterCrf.And(x => x.GROUP_LEVEL== dto.GroupLevel);

            return _vendorRepository.Get(queryFilterCrf, null, "").ToList();
        }
    }
}
