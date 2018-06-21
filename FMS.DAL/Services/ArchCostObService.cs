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
    public class ArchCostObService : IArchCostObService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_COST_OB> _archCostObRepository;

        public ArchCostObService(IUnitOfWork uow)
        {
            _uow = uow;
            _archCostObRepository = _uow.GetGenericRepository<ARCH_MST_COST_OB>();
        }

        public void Save(ARCH_MST_COST_OB db, Login userlogin)
        {
            try
            {
                _archCostObRepository.InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
            }
            catch (Exception exp)
            {

                throw new Exception(exp.Message);
            }
        }
        public List<ARCH_MST_COST_OB> GetCostObByFilter(CostObParamInput filter)
        {
            Expression<Func<ARCH_MST_COST_OB, bool>> queryFilter = PredicateHelper.True<ARCH_MST_COST_OB>();

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
            return _archCostObRepository.Get(queryFilter, null, "").ToList();
        }
    }
}
