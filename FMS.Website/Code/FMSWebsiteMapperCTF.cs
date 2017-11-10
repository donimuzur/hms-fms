using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject.Dto;
using FMS.Website.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FMS.Website.Code
{
    public partial class FMSWebsiteMapper
    {
        public static void InitializeCTF()
        {
            Mapper.CreateMap<TraCtfDto, CtfItem>().IgnoreAllNonExisting();
            Mapper.CreateMap<CtfItem, TraCtfDto>().IgnoreAllNonExisting();
        }
    }
}