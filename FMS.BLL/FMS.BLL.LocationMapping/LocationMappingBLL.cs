using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Dto;
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
        private IUnitOfWork _uow;
        public LocationMappingBLL(IUnitOfWork uow)
        {
            _uow = uow;
            _locationMappingService = new LocationMappingService(_uow);
        }

        public List<LocationMappingDto> GetLocationMapping()
        {
            var data = _locationMappingService.GetLocationMapping();
            var redata = Mapper.Map<List<LocationMappingDto>>(data);
            return redata;
        }

        public LocationMappingDto GetLocationMappingById(int MstLocationMappingId)
        {
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
