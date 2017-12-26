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
    public class CostObService : ICostObService
    {
        private IUnitOfWork _uow;

        private IGenericRepository<MST_COST_OB> _costObRepository;

        public CostObService(IUnitOfWork uow)
        {
            _uow = uow;
            _costObRepository = _uow.GetGenericRepository<MST_COST_OB>();
        }

        public List<MST_COST_OB> GetCostOb()
        {
            return _costObRepository.Get().ToList();
        }

        public MST_COST_OB GetCostObById(int MstCostObId)
        {
            return _costObRepository.GetByID(MstCostObId);
        }

        public MST_COST_OB GetExist(string Model)
        {
            return _costObRepository.Get(x => x.MODEL == Model).FirstOrDefault(); ;
        }

        public void save(MST_COST_OB dbCostOb)
        {
            _uow.GetGenericRepository<MST_COST_OB>().InsertOrUpdate(dbCostOb);
        }

        public void save(MST_COST_OB dbCostOb, Login userlogin)
        {
            _uow.GetGenericRepository<MST_COST_OB>().InsertOrUpdate(dbCostOb, userlogin, Enums.MenuList.MasterCostOB);
        }
        public List<MST_COST_OB> GetCostObByFilter(CostObParamInput filter)
        {
            Expression<Func<MST_COST_OB, bool>> queryFilter = PredicateHelper.True<MST_COST_OB>();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Function))
                {
                    queryFilter = queryFilter.And(c => (c.FUNCTION_NAME == null ? "" : c.FUNCTION_NAME.ToUpper()) == (filter.Function == null ? "" : filter.Function.ToUpper()));
                }
                if (!string.IsNullOrEmpty(filter.Regional))
                {
                    queryFilter = queryFilter.And(c => (c.REGIONAL == null ? "" : c.REGIONAL.ToUpper()) == (filter.Regional == null ? "" : filter.Regional.ToUpper()));
                }
                if (!string.IsNullOrEmpty(filter.VehicleType))
                {
                    queryFilter = queryFilter.And(c => (c.VEHICLE_TYPE == null ? "" : c.VEHICLE_TYPE.ToUpper()) == (filter.VehicleType == null ? "" : filter.VehicleType.ToUpper()));
                }
                if (filter.Status.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.IS_ACTIVE == filter.Status.Value);
                }
                if (filter.Year.HasValue)
                {
                    queryFilter = queryFilter.And(c => c.YEAR == filter.Year.Value);
                }
            }
            return _costObRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
