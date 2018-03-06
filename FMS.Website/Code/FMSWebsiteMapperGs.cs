using System;
using AutoMapper;
using FMS.AutoMapperExtensions;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FMS.BusinessObject.Inputs;
using FMS.Website.Models;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeGs()
        {
            Mapper.CreateMap<GSParamInput, GSSearchView>().IgnoreAllNonExisting();
            Mapper.CreateMap<GSSearchView, GSParamInput > ().IgnoreAllNonExisting();
        }
    }
}