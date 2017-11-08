using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;

namespace FMS.Contract.BLL
{
    public interface ICarComplaintFormBLL
    {
        List<CarComplaintFormDto> GetCCF();
        CarComplaintFormDto GetCCFByID(int Id);
        void Save(CarComplaintFormDto CCFDto);

        List<CarComplaintFormDto> GetFleetByEmployee(string EmployeeId);
        CarComplaintFormDto GetFleetByPoliceNumber(string PoliceNumber);
    }
}
