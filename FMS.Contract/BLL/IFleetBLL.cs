using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface IFleetBLL
    {
        List<FleetDto> GetFleet();
        FleetDto GetFleetById(int MstFleetId);
        void Save(FleetDto FleetDto);
        void Save(FleetDto FleetDto, Login userLogin);
        List<FleetDto> GetFleetByParam(FleetParamInput param);
        List<FleetDto> GetFleetForEndContractLessThan(int days);
        FleetDto GetVehicleByEmployeeId(string employeeId,string vehicleType);
    }
}
