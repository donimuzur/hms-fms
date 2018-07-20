using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Contract;
using FMS.Contract.BLL;
using FMS.Contract.Service;
using FMS.DAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.LocationMapping
{
    public class LocationMappingBLL : ILocationMappingBLL
    {

        private ILocationMappingService _locationMappingService;
        private IArchLocationMappingService _archLocationMappingService;
        private IUnitOfWork _uow;
        public LocationMappingBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _locationMappingService = new LocationMappingService(_uow);
            _archLocationMappingService = new ArchLocationMappingService(_uow);
        }

        public List<LocationMappingDto> GetLocationMapping()
        {
            var data = _locationMappingService.GetLocationMapping();
            var redata = Mapper.Map<List<LocationMappingDto>>(data);
            return redata;
        }
        public List<LocationMappingDto> GetLocationMapping(LocationMappingParamInput filter)
        {
            if(filter.Table == "2")
            {
                var archData = _archLocationMappingService.GetLocationMapping(filter);
                return Mapper.Map<List<LocationMappingDto>>(archData);
            }
            var data = _locationMappingService.GetLocationMapping(filter);
            var redata = Mapper.Map<List<LocationMappingDto>>(data);
            return redata;
        }
        public LocationMappingDto GetLocationMappingById(int MstLocationMappingId, bool? Archive = null)
        {
            if (Archive.HasValue)
            {
                var archData = _archLocationMappingService.GetLocationMappingById(MstLocationMappingId);
                return Mapper.Map<LocationMappingDto>(archData);
            }
            var data = _locationMappingService.GetLocationMappingById(MstLocationMappingId);
            var redata = Mapper.Map<LocationMappingDto>(data);
            return redata;
        }

        public void Save (LocationMappingDto Dto)
        {
            var db = Mapper.Map<MST_LOCATION_MAPPING>(Dto);
            _locationMappingService.Save(db);

        }
        public void SaveChanges()
        {
            _uow.SaveChanges();
        }
        public void Save(LocationMappingDto Dto, Login userLogin)
        {
            var db = Mapper.Map<MST_LOCATION_MAPPING>(Dto);
            _locationMappingService.Save(db, userLogin);

        }
    }
}
