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
        List<CarComplaintFormDtoDetil> GetCCFD1();
        CarComplaintFormDto GetCCFByID(int Id);
        void Save(CarComplaintFormDto CCFDto, CarComplaintFormDtoDetil CCFDtoD1);

        List<CarComplaintFormDto> GetFleetByEmployee(string EmployeeId);
        //CarComplaintFormDto GetFleetByPoliceNumber(string PoliceNumber);
    }
}
