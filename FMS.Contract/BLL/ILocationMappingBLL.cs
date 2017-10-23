﻿using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.Contract.BLL
{
    public interface ILocationMappingBLL
    {
        LocationMappingDto GetLocationMappingById(int MstLocationMappingId);
        List<LocationMappingDto> GetLocationMapping();
        void Save(LocationMappingDto Dto);
    }
}
