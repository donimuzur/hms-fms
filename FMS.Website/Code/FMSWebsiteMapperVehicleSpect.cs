using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Inputs;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeVehicleSpect()
        {
            Mapper.CreateMap<VehicleSpectParamInput, VehicleSpectSearchView>().IgnoreAllNonExisting();
            Mapper.CreateMap<VehicleSpectSearchView, VehicleSpectParamInput>().IgnoreAllNonExisting();
        }
    }
}