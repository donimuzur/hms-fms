using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.BLL
{
    public interface IEmployeeBLL
    {
        List<EmployeeDto> GetEmployee();
        EmployeeDto GetExist(string FormalName);
        EmployeeDto GetByID(string Id);
        void Save(EmployeeDto EmployeeDto);

        List<EmployeeLocationDto> GetCityLocation();

        List<EmployeeLocationDto> GetEmployeeCityList();
        List<EmployeeLocationDto> GetLocationByCity(string city);

        string GetCityByLocation(string location);

        List<EmployeeLocationDto> GetLocationAll();
    }
}
