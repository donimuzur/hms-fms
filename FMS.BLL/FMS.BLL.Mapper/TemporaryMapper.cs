﻿using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.BusinessObject.Inputs;

namespace FMS.BLL.Mapper
{
    public class TemporaryMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<TemporaryDto, TRA_TEMPORARY>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DOCUMENT_NUMBER, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER_TEMP));

            AutoMapper.Mapper.CreateMap<TRA_TEMPORARY, TemporaryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DOCUMENT_NUMBER_TEMP, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER));

            AutoMapper.Mapper.CreateMap<TraCsfDto, TemporaryDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.DOCUMENT_NUMBER_RELATED, opt => opt.MapFrom(src => src.DOCUMENT_NUMBER));
        }
    }
}
