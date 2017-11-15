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
using FMS.BusinessObject.Business;

namespace FMS.BLL.VehicleSpect
{
    public class VehicleSpectBLL : IVehicleSpectBLL
    {
        private IVehicleSpectService _VehicleSpectService;
        private IUnitOfWork _uow;
        public VehicleSpectBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _VehicleSpectService = new VehicleSpectService(uow);
        }

        public List<VehicleSpectDto> GetVehicleSpect()
        {
            var data = _VehicleSpectService.GetVehicleSpect();
            var retData = Mapper.Map<List<VehicleSpectDto>>(data);
            return retData;
        }

        public VehicleSpectDto GetVehicleSpectById(int Id)
        {
            var data = _VehicleSpectService.GetVehicleSpectById(Id);
            var retData = Mapper.Map<VehicleSpectDto>(data);

            return retData;
        }

        public void Save(VehicleSpectDto VehicleSpectDto)
        {
            var dbVehicleSpect = Mapper.Map<MST_VEHICLE_SPECT>(VehicleSpectDto);
            _VehicleSpectService.save(dbVehicleSpect);
        }

        public void Save(VehicleSpectDto VehicleSpectDto, Login userLogin)
        {
            var dbVehicleSpect = Mapper.Map<MST_VEHICLE_SPECT>(VehicleSpectDto);
            _VehicleSpectService.save(dbVehicleSpect, userLogin);
        }
    }
}
