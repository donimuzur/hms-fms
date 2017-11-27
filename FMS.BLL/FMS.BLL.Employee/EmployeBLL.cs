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
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.Employee
{
    public class EmployeBLL : IEmployeeBLL
    {
        //private ILogger _logger;
        private IEmployeeService _employeeService;
        //private IRoleService _roleService;
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



        public List<EmployeeLocationDto> GetCityLocation()
        {
            var data = _employeeService.GetEmployee().GroupBy(x=> new { x.BASETOWN, x.CITY}).Select(x=> new EmployeeLocationDto()
            {
                City = x.Key.CITY,
                Location = x.Key.BASETOWN
            }).ToList();
            return data;
        }

        public List<EmployeeLocationDto> GetEmployeeCityList()
        {
            var data = GetCityLocation().GroupBy(x => x.City).Select(x => new EmployeeLocationDto()
            {
                City = x.Key
                
            }).ToList();
            return data;
        }

        public List<EmployeeLocationDto> GetLocationByCity(string city)
        {
            var data = GetCityLocation().Where(x=> x.City == city).Select(x => new EmployeeLocationDto()
            {
                Location = x.Location

            }).ToList();
            return data;
        }

        public List<EmployeeLocationDto> GetLocationAll()
        {
            var data = GetCityLocation().GroupBy(x => x.Location ).Select(x => new EmployeeLocationDto()
            {
                Location = x.Key

            }).ToList();
            return data;
        }

        public string GetCityByLocation(string location)
        {
            var data = GetCityLocation().Where(x => x.Location == location).FirstOrDefault();
            if (data != null) return data.City;
            else return null;
        }


        public List<EmployeeDto> GetEmployeeByParam(EmployeeParamInput param)
        {
            var data = _employeeService.GetEmployeeByParam(param);
            return Mapper.Map<List<EmployeeDto>>(data);
        }
    }


}
