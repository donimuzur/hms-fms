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
        private IUnitOfWork _uow;
        public FleetBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _FleetService = new FleetService(uow);
        }
        
        public List<FleetDto> GetFleet()
        {
            var data = _FleetService.GetFleet();
            var redata = Mapper.Map<List<FleetDto>>(data);
            return redata;
        }


        public List<FleetDto> GetFleet(int pageNumber, int dataPerPage)
        {
            var data = _FleetService.GetFleet(pageNumber,dataPerPage);
            var redata = Mapper.Map<List<FleetDto>>(data);
            return redata;
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

        public FleetDto GetFleetById(int MstFleetId)
        {
            var db = _FleetService.GetFleetById(MstFleetId);
            var data = Mapper.Map<FleetDto>(db);
            return data;
        }

        public FleetDto GetVehicleByEmployeeId(string employeeId,string vehicleType)
        {
            var db = _FleetService.GetFleetByParam(new FleetParamInput()
            {
                EmployeeId = employeeId,
                VehicleType = vehicleType
                
            }).FirstOrDefault();
            var data = Mapper.Map<FleetDto>(db);
            return data;

        }
        public List<FleetDto> GetFleetForEndContractLessThan(int days)
        {
            var data = _FleetService.GetFleetForEndContractLessThan(days);
            var redata = Mapper.Map<List<FleetDto>>(data);
            return redata;
        }
    }
}
