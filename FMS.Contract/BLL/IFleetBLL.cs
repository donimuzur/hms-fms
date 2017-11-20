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

        FleetDto GetVehicleByEmployeeId(string employeeId);
        List<FleetDto> GetFleetByParam(FleetSearchInput param);
    }
}
