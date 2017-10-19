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

        public void Save(FleetDto FleetDto)
        {
            var dbFleet = Mapper.Map<MST_FLEET>(FleetDto);
            _FleetService.save(dbFleet);
        }

        public FleetDto GetFleetById(int MstFleetId)
        {
            var db = _FleetService.GetFleetById(MstFleetId);
            var data = Mapper.Map<FleetDto>(db);
            return data;
        }
    }
}
