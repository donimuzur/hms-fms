using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using FMS.DAL.Services;
using FMS.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FMS.DAL.Services
{
    public class ArchEmployeeService : IArchEmployeeService
    {
        private IUnitOfWork _uow;
        private IGenericRepository<ARCH_MST_EMPLOYEE> _archEmployeeRepository;
        public ArchEmployeeService(IUnitOfWork uow)
        {
            _uow = uow;
            _archEmployeeRepository = uow.GetGenericRepository<ARCH_MST_EMPLOYEE>();
        }
        public void Save(ARCH_MST_EMPLOYEE db, Login userlogin)
        {
            _uow.GetGenericRepository<ARCH_MST_EMPLOYEE>().InsertOrUpdate(db, userlogin, Enums.MenuList.MasterData);
        }
        public List<ARCH_MST_EMPLOYEE> GetEmployeeByParam(EmployeeParamInput input)
        {
            Expression<Func<ARCH_MST_EMPLOYEE, bool>> queryFilterEmployee = PredicateHelper.True<ARCH_MST_EMPLOYEE>();
            queryFilterEmployee = c => 1 == 1;
            if (input.Status != null)
            {
                queryFilterEmployee = c => c.IS_ACTIVE == false;
            }

            if (input != null && queryFilterEmployee != null)
            {
                if (!string.IsNullOrEmpty(input.Address))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.ADDRESS.Contains(input.Address));

                }

                if (!string.IsNullOrEmpty(input.BaseTown))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.BASETOWN.Contains(input.BaseTown));

                }

                if (!string.IsNullOrEmpty(input.City))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.CITY.Contains(input.City));

                }

                if (!string.IsNullOrEmpty(input.Company))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.COMPANY.Contains(input.Company));

                }

                if (!string.IsNullOrEmpty(input.CostCenter))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.COST_CENTER.Contains(input.CostCenter));

                }

                if (!string.IsNullOrEmpty(input.Directorate))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.DIRECTORATE.Contains(input.Directorate));

                }

                if (!string.IsNullOrEmpty(input.Division))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.DIVISON.Contains(input.Division));

                }

                if (!string.IsNullOrEmpty(input.EmailAddress))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.EMAIL_ADDRESS.Contains(input.EmailAddress));

                }

                if (!string.IsNullOrEmpty(input.EmployeeId))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.EMPLOYEE_ID.Contains(input.EmployeeId));

                }

                if (!string.IsNullOrEmpty(input.FlexPoint))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.FLEX_POINT == Convert.ToInt32(input.FlexPoint));

                }

                if (!string.IsNullOrEmpty(input.FormalName))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.FORMAL_NAME.Contains(input.FormalName));

                }

                if (!string.IsNullOrEmpty(input.GroupLevel))
                {
                    var grpLevel = Convert.ToInt32(input.GroupLevel);
                    queryFilterEmployee = queryFilterEmployee.And(c => c.GROUP_LEVEL == grpLevel);

                }

                if (!string.IsNullOrEmpty(input.PositionTitle))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.POSITION_TITLE.Contains(input.PositionTitle));

                }


            }
            return _archEmployeeRepository.Get(queryFilterEmployee, null, "").ToList();
        }
        public ARCH_MST_EMPLOYEE GetEmployeeById(string MstEmployeeId)
        {
            return _archEmployeeRepository.GetByID(MstEmployeeId);
        }

    }
}
