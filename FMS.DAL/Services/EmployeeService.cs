using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.Contract;
using FMS.Contract.Service;
using FMS.Core;
using NLog;

namespace FMS.DAL.Services
{
    public class EmployeeService : IEmployeeService
    {
        private ILogger _logger;
        private IGenericRepository<employee> _employeeRepository;
        private string _includeTables;

        public EmployeeService(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _employeeRepository = uow.GetGenericRepository<employee>();
        }

        public void GetEmployeeFromPeopleSoft()
        {
            throw new NotImplementedException();
        }

        public Login VerifyLogin(string accountName)
        {
            var user = _employeeRepository.Get(x => x.acountname.ToLower() == accountName, null, _includeTables).FirstOrDefault();
            if (user != null)
            {
                return new Login()
                {
                    FIRST_NAME = user.employee_name,
                    LAST_NAME = user.employee_name,
                    USERNAME = user.acountname,
                    USER_ID = user.acountname,
                    //UserRole = user.role

                };
            }
            else
            {
                return null;
                
            }
        }
    }
}
