using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;
using FMS.Website.Models;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeRptFuel()
        {
            Mapper.CreateMap<RptFuelDto, RptFuelItem>().IgnoreAllNonExisting()
             .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate == null ? DateTime.Now : src.CreatedDate));

            Mapper.CreateMap<RptFuelSearchView, RptFuelByParamInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<RptFuelByParamInput, RptFuelSearchView>().IgnoreAllNonExisting();

            Mapper.CreateMap<RptFuelSearchViewExport, RptFuelByParamInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<RptFuelByParamInput, RptFuelSearchViewExport>().IgnoreAllNonExisting();
        }
    }
}