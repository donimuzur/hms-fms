using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;
using FMS.BusinessObject.Business;
using FMS.Core;
using FMS.BusinessObject.Inputs;
using System.Linq.Expressions;
using FMS.Utils;

namespace FMS.DAL.Services
{
    public class PenaltyService : IPenaltyService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_PENALTY> _penaltyRepository;

        public PenaltyService(IUnitOfWork uow)
        {
            _uow = uow;

            _penaltyRepository = _uow.GetGenericRepository<MST_PENALTY>();
        }
        public List<MST_PENALTY> GetPenalty()
        {
            return _penaltyRepository.Get().ToList();
        }

        public MST_PENALTY GetPenaltyById(int MstPenaltyID)
        {
            return _penaltyRepository.GetByID(MstPenaltyID);
        }

        public void save(MST_PENALTY dbPenalty)
        {
            _penaltyRepository.InsertOrUpdate(dbPenalty);
            
        }
        public void save(MST_PENALTY dbPenalty, Login userLogin)
        {
            _uow.GetGenericRepository<MST_PENALTY>().InsertOrUpdate(dbPenalty, userLogin, Enums.MenuList.MasterRemark);
        }

        public List<MST_PENALTY> GetPenalty(PenaltyParamInput filter)
        {
            Expression<Func<MST_PENALTY, bool>> queryFilter = c => c.IS_ACTIVE;

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
                    var listFunction = filter. Model.ToUpper().Split(',').ToList();
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
            
            return _penaltyRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
