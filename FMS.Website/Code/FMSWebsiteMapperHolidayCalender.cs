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
        public static void InitializeHolidayCalender()
        {
            Mapper.CreateMap<HolidayCalenderParamInput, HolidayCalenderSearchView>().IgnoreAllNonExisting();
            Mapper.CreateMap<HolidayCalenderSearchView, HolidayCalenderParamInput>().IgnoreAllNonExisting();
        }
    }
}