using FMS.BusinessObject.Inputs;
using FMS.Contract.BLL;
using FMS.BusinessObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.Contract.Service;
using FMS.Contract;
using FMS.DAL.Services;
using FMS.BusinessObject.Dto;
using AutoMapper;
using FMS.BusinessObject.Business;

namespace FMS.BLL.Fleet
{
    public class FleetBLL : IFleetBLL
    {
        //private ILogger _logger;
        private IFleetService _FleetService;
        private IArchFleetService _archFleetService;
        private IUnitOfWork _uow;
        public FleetBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _FleetService = new FleetService(_uow);
            _archFleetService = new ArchFleetService(_uow);
        }
        
        public List<FleetDto> GetFleet(FleetParamInput input = null)
        {
            var retData = new List<FleetDto>();

            if (input != null && input.Table == "2")
            {
                var data = _archFleetService.GetFleet();
                retData = Mapper.Map<List<FleetDto>>(data);
            }
            else
            {
                var data = _FleetService.GetFleet();
                retData = Mapper.Map<List<FleetDto>>(data);
            }
            return retData;
        }

        public void Save(FleetDto FleetDto)
        {
            var dbFleet = Mapper.Map<MST_FLEET>(FleetDto);
            _FleetService.save(dbFleet);
        }

        public void Save(FleetDto FleetDto, Login userLogin)
        {
            var dbFleet = Mapper.Map<MST_FLEET>(FleetDto);
            _FleetService.save(dbFleet, userLogin);
        }

        public FleetDto GetFleetById(int MstFleetId,bool? Archived = null)
        {
            var retData = new FleetDto();

            if (Archived.HasValue)
            {
                var data = _archFleetService.GetFleetById(MstFleetId);
                retData = Mapper.Map<FleetDto>(data);
            }
            else
            {
                var data = _FleetService.GetFleetById(MstFleetId);
                retData = Mapper.Map<FleetDto>(data);
            }
            return retData;
        }

        public FleetDto GetVehicleByEmployeeId(string employeeId, string vehicleType)
        {
            var db = _FleetService.GetFleetByParam(new FleetParamInput()
            {
                EmployeeId = employeeId,
                VehicleType = vehicleType,
                Status = "True"

            }).FirstOrDefault();
            var data = Mapper.Map<FleetDto>(db);
            return data;

        }
        public FleetDto GetVehicleByEmployeeId(string employeeId)
        {
            var db = _FleetService.GetFleetByParam(new FleetParamInput()
            {
                EmployeeId = employeeId

            }).FirstOrDefault();
            var data = Mapper.Map<FleetDto>(db);
            return data;

        }

        public List<FleetDto> GetFleetByParam(FleetParamInput param)
        {
            var retData = new List<FleetDto>();
            if(param.Table == "2")
            {
                var data = _archFleetService.GetFleetByParam(param);
                retData = Mapper.Map<List<FleetDto>>(data);
            }
            else
            {
                var data = _FleetService.GetFleetByParam(param);
                retData = Mapper.Map<List<FleetDto>>(data);
            }
            return retData;
        }

        public List<FleetDto> GetFleetForEndContractLessThan(int days)
        {
            var data = _FleetService.GetFleetForEndContractLessThan(days);
            var redata = Mapper.Map<List<FleetDto>>(data);
            return redata;
        }

        public List<FleetChangeDto> GetFleetChange()
        {
            var data = _FleetService.GetFleetChange();
            var redata = Mapper.Map<List<FleetChangeDto>>(data);
            return redata;
        }

        public List<FleetChangeDto> GetFleetChangeByParam(FleetChangeParamInput param)
        {
            var data = _FleetService.GetFleetChangeByParam(param);
            return Mapper.Map<List<FleetChangeDto>>(data);
        }

        public bool UpdateFleetChange(string fleetChangeList, Login userLogin)
        {
            var isSuccess = false;

            try
            {
                var listFleetChange = fleetChangeList.Split(',').ToList();
                var data = GetFleetChange();

                foreach (var item in listFleetChange)
                {
                    var id = Convert.ToInt64(item);

                    var fleetChange = data.Where(x => x.FleetChangeId == id).FirstOrDefault();

                    if (fleetChange != null)
                    {
                        var idFleet = Convert.ToInt32(fleetChange.FleetId);

                        var fleetData = GetFleetById(idFleet);

                        switch (fleetChange.FieldName)
                        {
                            case "COST CENTER":
                                fleetData.CostCenter = fleetChange.DataAfter;
                                break;
                            case "BASE TOWN":
                                fleetData.City = fleetChange.DataAfter;
                                break;
                            case "EMPLOYEE NAME":
                                fleetData.EmployeeName = fleetChange.DataAfter;
                                break;
                        }

                        fleetChange.ModifiedBy = userLogin.USER_ID;
                        fleetChange.ModifiedDate = DateTime.Now;

                        fleetData.ModifiedBy = userLogin.USER_ID;
                        fleetData.ModifiedDate = DateTime.Now;

                        Save(fleetData, userLogin);
                        SaveFleetChange(fleetChange, userLogin);
                    }
                }
            }
            catch(Exception message)
            {
                var error = message.Message;
            }
            return isSuccess;
        }

        public void SaveFleetChange(FleetChangeDto FleetChangeDto, Login userLogin)
        {
            var dbFleetChange = Mapper.Map<FLEET_CHANGE>(FleetChangeDto);
            _FleetService.saveFleetChange(dbFleetChange, userLogin);
        }
    }
}
