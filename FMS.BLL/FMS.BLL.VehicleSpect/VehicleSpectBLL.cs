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
using FMS.BusinessObject.Inputs;

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
            foreach(VehicleSpectDto item in retData)
            {
                item.FuelTypeSpect = data.Where(x => x.MST_VEHICLE_SPECT_ID == item.MstVehicleSpectId).First().FUEL_TYPE;
            }
            return retData;
        }

        public VehicleSpectDto GetVehicleSpectById(int Id)
        {
            var data = _VehicleSpectService.GetVehicleSpectById(Id);
            var retData = Mapper.Map<VehicleSpectDto>(data);
            retData.FuelTypeSpect = data.FUEL_TYPE;

            return retData;
        }

        public void Save(VehicleSpectDto VehicleSpectDto)
        {
            var dbVehicleSpect = Mapper.Map<MST_VEHICLE_SPECT>(VehicleSpectDto);
            dbVehicleSpect.FUEL_TYPE = VehicleSpectDto.FuelTypeSpect;
            _VehicleSpectService.save(dbVehicleSpect);
        }

        public void Save(VehicleSpectDto VehicleSpectDto, Login userLogin)
        {
            var dbVehicleSpect = Mapper.Map<MST_VEHICLE_SPECT>(VehicleSpectDto);
            dbVehicleSpect.FUEL_TYPE = VehicleSpectDto.FuelTypeSpect;
            _VehicleSpectService.save(dbVehicleSpect, userLogin);
        }



        public void ValidateSpect(VehicleSpectDto dto, out string message,bool isEdit = false)
        {
            message = "";
            List<string> messageList = new List<string>();
            if (string.IsNullOrEmpty(dto.Manufacturer))
            {
                messageList.Add("Manufacturer must not be empty");
            }

            if (string.IsNullOrEmpty(dto.Models))
            {
                messageList.Add("Model must not be empty");
            }

            if (dto.Year == 0)
            {
                messageList.Add("Year must not be 0 or null");
            }

            if (string.IsNullOrEmpty(dto.BodyType))
            {
                messageList.Add("Body type must not be empty");
            }

            if (string.IsNullOrEmpty(dto.Color))
            {
                messageList.Add("Color must not be empty");
            }

            if (string.IsNullOrEmpty(dto.FuelTypeSpect))
            {
                messageList.Add("Fuel type must not be empty");
            }

            if (string.IsNullOrEmpty(dto.Series))
            {
                messageList.Add("Series must not be empty");
            }

            if (string.IsNullOrEmpty(dto.Transmission))
            {
                messageList.Add("Transmission must not be empty");
            }


            if (!isEdit)
            {
                if (string.IsNullOrEmpty(message))
                {
                    List<MST_VEHICLE_SPECT> data = _VehicleSpectService.GetExistingVehicleSpectByParam(dto);
                    if (data.Count > 0)
                    {
                        messageList.Add("Similar data exist.");
                    }
                }   
            }
            

            var index = 0;
            foreach (var msg in messageList)
            {

                if (index == 0)
                {
                    message = msg;
                }
                else
                {
                    message = message + "," + msg;
                }

                index++;
            }

            
        }

        public List<VehicleSpectDto> GetVehicleSpect(VehicleSpectParamInput filter)
        {
            var data = _VehicleSpectService.GetVehicleSpect(filter);
            var retData = Mapper.Map<List<VehicleSpectDto>>(data);
            foreach (VehicleSpectDto item in retData)
            {
                item.FuelTypeSpect = data.Where(x => x.MST_VEHICLE_SPECT_ID == item.MstVehicleSpectId).First().FUEL_TYPE;
            }
            return retData;
        }
    }
}
