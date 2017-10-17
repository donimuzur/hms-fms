using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Contract;
using FMS.DAL.Services;
using AutoMapper;


namespace FMS.BLL.Employee
{
    public class EmployeBLL : IEmployeeBLL
    {
        //private ILogger _logger;
        private IEmployeeService _employeeService;
        private IRoleService _roleService;
        private IUnitOfWork _uow;

        public EmployeBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _employeeService = new EmployeeService(uow);
        }

        public List<EmployeeDto> GetEmployee()
        {
            var data = _employeeService.GetEmployee();
            var retData = Mapper.Map<List<EmployeeDto>>(data);
            return retData;
        }

        public EmployeeDto GetByID(string Id)
        {
            var data = _employeeService.GetEmployeeById(Id);
            var retData = Mapper.Map<EmployeeDto>(data);

            return retData;
        }

        public EmployeeDto GetExist(string FormalName)
        {
            var data = _employeeService.GetExist(FormalName);
            var retData = Mapper.Map<EmployeeDto>(data);

            return retData;
        }
        public void Save(EmployeeDto EmployeeDto)
        {
            var dbEmployee = Mapper.Map<MST_EMPLOYEE>(EmployeeDto);
            _uow.GetGenericRepository<MST_EMPLOYEE>().InsertOrUpdate(dbEmployee);
            _uow.SaveChanges();
        }

    }


}
