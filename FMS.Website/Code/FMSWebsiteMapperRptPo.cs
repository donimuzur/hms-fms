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
    public partial class FMSWebsiteMapperRptPo
    {
        public static void InitializeRptPo()
        {
            Mapper.CreateMap<RptPODto, RptPOItem>().IgnoreAllNonExisting();

        }
    }
}