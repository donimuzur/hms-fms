using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeExecutiveSummary()
        {
            Mapper.CreateMap<NoVehicleDto, NoVehicleData>().IgnoreAllNonExisting();
        }
    }
}