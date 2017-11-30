using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;
using FMS.BusinessObject.Inputs;
using System.Linq.Expressions;
using FMS.Utils;

namespace FMS.DAL.Services
{
    public class EmployeeService : IEmployeeService
    {
        private IUnitOfWork _uow;
        
        private IGenericRepository<MST_EMPLOYEE> _employeeRepository;

        public EmployeeService(IUnitOfWork uow)
        {
            _uow = uow;

            _employeeRepository = _uow.GetGenericRepository<MST_EMPLOYEE>();
        }
        public List<MST_EMPLOYEE> GetEmployee()
        {
            return _employeeRepository.Get().ToList();
        }


        public List<MST_EMPLOYEE> GetEmployeeByParam(EmployeeParamInput input)
        {
            Expression<Func<MST_EMPLOYEE, bool>> queryFilterEmployee = null;
            if(input.Status != null)
            {
                queryFilterEmployee = c => c.IS_ACTIVE == input.Status;
            }
            
            if (input != null)
            {
                if (!string.IsNullOrEmpty(input.Address))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.ADDRESS == input.Address);

                }

                if (!string.IsNullOrEmpty(input.BaseTown))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.BASETOWN == input.BaseTown);

                }

                if (!string.IsNullOrEmpty(input.City))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.CITY == input.City);

                }

                if (!string.IsNullOrEmpty(input.Company))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.COMPANY == input.Company);

                }

                if (!string.IsNullOrEmpty(input.CostCenter))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.COST_CENTER == input.CostCenter);

                }

                if (!string.IsNullOrEmpty(input.Directorate))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.DIRECTORATE == input.Directorate);

                }

                if (!string.IsNullOrEmpty(input.Division))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.DIVISON == input.Division);

                }

                if (!string.IsNullOrEmpty(input.EmailAddress))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.EMAIL_ADDRESS == input.EmailAddress);

                }

                if (!string.IsNullOrEmpty(input.EmployeeId))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.EMPLOYEE_ID == input.EmployeeId);

                }

                if (!string.IsNullOrEmpty(input.FlexPoint))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.FLEX_POINT == Convert.ToInt32(input.FlexPoint));

                }

                if (!string.IsNullOrEmpty(input.FormalName))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.FORMAL_NAME == input.FormalName);

                }

                if (!string.IsNullOrEmpty(input.GroupLevel))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.GROUP_LEVEL == Convert.ToInt32(input.Company));

                }

                if (!string.IsNullOrEmpty(input.PositionTitle))
                {
                    queryFilterEmployee = queryFilterEmployee.And(c => c.POSITION_TITLE == input.PositionTitle);

                }


            }
            return _employeeRepository.Get(queryFilterEmployee, null, "").ToList();
        }

        public MST_EMPLOYEE GetEmployeeById(string MstEmployeeId)
        {
            return _employeeRepository.GetByID(MstEmployeeId);
        }

        public MST_EMPLOYEE GetExist(string FormalName)
        {
            return _employeeRepository.Get(x => x.FORMAL_NAME == FormalName).FirstOrDefault(); ;

        }

        public string GetLastEmployeeId()
        {
            return _employeeRepository.Get().OrderByDescending(x => x.EMPLOYEE_ID).First().EMPLOYEE_ID;
        }

    }
}
