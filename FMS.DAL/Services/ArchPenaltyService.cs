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

        public List<ARCH_MST_PENALTY> GetPenalty(PenaltyParamInput filter)
        {
            Expression<Func<ARCH_MST_PENALTY, bool>> queryFilter = c => c.IS_ACTIVE == true;

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
                if (!string.IsNullOrEmpty(filter.RequestYear))
                {
                    var listFunction = filter.RequestYear.Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.YEAR == null ? "" : c.YEAR.ToString())));
                }
                if (!string.IsNullOrEmpty(filter.Series))
                {
                    var listFunction = filter.Series.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.SERIES == null ? "" : c.SERIES.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.Vendor))
                {
                    var listFunction = filter.Vendor.Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.VENDOR == null ? "" : c.VENDOR.ToString())));
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    var listFunction = filter.VehicleType.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.VEHICLE_TYPE == null ? "" : c.VEHICLE_TYPE.ToUpper())));
                }
            }

            return _archPenaltyRepository.Get(queryFilter, null, "").ToList();
        }
        public ARCH_MST_PENALTY GetPenaltyById(int id)
        {
            return _archPenaltyRepository.GetByID(id);
        }
    }
}
