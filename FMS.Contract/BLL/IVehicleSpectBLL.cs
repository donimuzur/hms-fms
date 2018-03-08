using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
   public interface IVehicleSpectBLL
    {
        List<VehicleSpectDto> GetVehicleSpect();
        VehicleSpectDto GetVehicleSpectById(int Id);
        void Save(VehicleSpectDto VehicleSpectDto);
        void Save(VehicleSpectDto data, Login currentUser);

        void ValidateSpect(VehicleSpectDto dto, out string message, bool isEdit = false);
        List<VehicleSpectDto> GetVehicleSpect(VehicleSpectParamInput vehicleSpectParamInput);
    }
}
