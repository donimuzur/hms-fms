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
        public static void InitializeRptCCF()
        {
            Mapper.CreateMap<RptCCFDto, RptCCFItem>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.ModifiedDate == null ? src.CreatedDate : src.ModifiedDate));

            Mapper.CreateMap<RptCCFSearchView, RptCCFInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<RptCCFInput, RptCCFSearchView>().IgnoreAllNonExisting();

            Mapper.CreateMap<RptCCFSearchViewExport, RptCCFInput>().IgnoreAllNonExisting();
            Mapper.CreateMap<RptCCFInput, RptCCFSearchViewExport>().IgnoreAllNonExisting();
        }
    }
}