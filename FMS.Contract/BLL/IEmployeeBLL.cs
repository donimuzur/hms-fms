using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface IEmployeeBLL
    {
        List<EmployeeDto> GetEmployee();
        EmployeeDto GetExist(string FormalName);
        EmployeeDto GetByID(string Id, bool? Archived = null);
        void Save(EmployeeDto EmployeeDto);

        List<EmployeeLocationDto> GetCityLocation();

        List<EmployeeLocationDto> GetEmployeeCityList();
        List<EmployeeLocationDto> GetLocationByCity(string city);

        string GetCityByLocation(string location);

        List<EmployeeLocationDto> GetLocationAll();
        List<EmployeeDto> GetEmployeeByParam(EmployeeParamInput param);
        string GetLastEmployeeId();
    }
}
