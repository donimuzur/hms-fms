using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;

namespace FMS.Website.Code
{
    public class FMSWebsiteMapperRptFuel
    {
        public static void InitializeRptFuel()
        {
            Mapper.CreateMap<RptFuelDto, RptFuelItem>().IgnoreAllNonExisting()
             .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate == null ? DateTime.Now : src.CreatedDate));
        }
    }
}