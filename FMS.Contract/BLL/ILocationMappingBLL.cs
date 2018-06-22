using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMS.BusinessObject.Business;
using FMS.BusinessObject.Inputs;

namespace FMS.Contract.BLL
{
    public interface ILocationMappingBLL
    {
        LocationMappingDto GetLocationMappingById(int MstLocationMappingId, bool? Archive = null);
        List<LocationMappingDto> GetLocationMapping(LocationMappingParamInput filter);
        List<LocationMappingDto> GetLocationMapping();
        void Save(LocationMappingDto Dto);
        void Save(LocationMappingDto data, Login currentUser);
        void SaveChanges();
    }
}
