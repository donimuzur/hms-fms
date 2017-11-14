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
        FleetDto GetFleetById(int MstFleetId);
        void Save(FleetDto FleetDto);

        
    }
}
