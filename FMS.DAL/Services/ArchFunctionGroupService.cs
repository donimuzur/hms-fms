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
    public class ArchFunctionGroupService : IArchFunctionGroupService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_FUNCTION_GROUP> _archFunctionGroupRepository;
        public ArchFunctionGroupService(IUnitOfWork uow)
        {
            _uow = uow;
            _archFunctionGroupRepository = uow.GetGenericRepository<ARCH_MST_FUNCTION_GROUP>();
        }
        public void Save(ARCH_MST_FUNCTION_GROUP db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_FUNCTION_GROUP>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        public List<ARCH_MST_FUNCTION_GROUP> GetGroupCostCenter()
        {
            return _archFunctionGroupRepository.Get().ToList();
        }
        public List<ARCH_MST_FUNCTION_GROUP> GetGroupCostCenter(GroupCostCenterParamInput filter)
        {
            Expression<Func<ARCH_MST_FUNCTION_GROUP, bool>> queryFilter= PredicateHelper.True<ARCH_MST_FUNCTION_GROUP>();

            if (filter != null)
            {

                if (!string.IsNullOrEmpty(filter.Function))
                {
                    var listFunction = filter.Function.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.FUNCTION_NAME == null ? "" : c.FUNCTION_NAME.ToUpper())));
                }
                if (!string.IsNullOrEmpty(filter.CostCenter))
                {
                    var listFunction = filter.CostCenter.ToUpper().Split(',').ToList();
                    queryFilter = queryFilter.And(c => listFunction.Contains((c.COST_CENTER == null ? "" : c.COST_CENTER.ToUpper())));
                }
            }

            return _archFunctionGroupRepository.Get(queryFilter, null, "").ToList();
        }
        public ARCH_MST_FUNCTION_GROUP GetGroupCostCenterById(int MstGroupCostCenterId)
        {
            return _archFunctionGroupRepository.GetByID(MstGroupCostCenterId);
        }
    }
}
