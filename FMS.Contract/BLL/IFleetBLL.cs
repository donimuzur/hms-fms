using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface IFleetBLL
    {
        List<FleetDto> GetFleet();
        List<FleetDto> GetFleet(int pageNumber, int dataPerPage);
        FleetDto GetFleetById(int MstFleetId);
        void Save(FleetDto FleetDto);
        void Save(FleetDto FleetDto, Login userLogin);
        List<FleetDto> GetFleetForEndContractLessThan(int days);
        FleetDto GetVehicleByEmployeeId(string employeeId,string vehicleType);
    }
}
