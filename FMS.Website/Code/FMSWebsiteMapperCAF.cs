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

        public static void InitializeCAF()
        {
            Mapper.CreateMap<TraCafDto, TraCafItemDetails>().IgnoreAllNonExisting()
                .ForMember(dest => dest.ProgressDetails, opt => opt.MapFrom(src => AutoMapper.Mapper.Map<List<TraCafProgress>>(src.ProgressDetails)));

            Mapper.CreateMap<TraCafItemDetails, TraCafDto>().IgnoreAllNonExisting();

            Mapper.CreateMap<TraCafProgressDto, TraCafProgress>().IgnoreAllNonExisting()
                ;
            Mapper.CreateMap<TraCafProgress, TraCafProgressDto>().IgnoreAllNonExisting();

        }
        
    }
}