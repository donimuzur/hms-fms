using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject;
using FMS.Contract;
using FMS.Contract.Service;
using NLog;

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

        public MST_EMPLOYEE GetEmployeeById(string MstEmployeeId)
        {
            return _employeeRepository.GetByID(MstEmployeeId);
        }

        public MST_EMPLOYEE GetExist(string FormalName)
        {
            return _employeeRepository.Get(x => x.FORMAL_NAME == FormalName).FirstOrDefault(); ;

        }

    }
}
