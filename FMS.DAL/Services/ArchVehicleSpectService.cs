using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public List<ARCH_MST_VEHICLE_SPECT> GetVehicleSpect(VehicleSpectParamInput filter)
        {
            Expression<Func<ARCH_MST_VEHICLE_SPECT, bool>> queryFilter = PredicateHelper.True<ARCH_MST_VEHICLE_SPECT>(); ;

            if (filter != null)
            {

                if (!string.IsNullOrEmpty(filter.BodyType))
                {
                    var listFunction = filter.BodyType.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.BODY_TYPE == null ? "" : c.BODY_TYPE.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.Manufacturer))
                {
                    var listFunction = filter.Manufacturer.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.MANUFACTURER == null ? "" : c.MANUFACTURER.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.Model))
                {
                    var listFunction = filter.Model.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.MODEL == null ? "" : c.MODEL.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.Series))
                {
                    var listFunction = filter.Series.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.SERIES == null ? "" : c.SERIES.ToUpper())));
                }
            }

            return _archVehicleSpectRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
