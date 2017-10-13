using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;

namespace FMS.BLL.Mapper
{
    public class ComplaintMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_COMPLAINT_CATEGORY, ComplaintDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompCatId, opt => opt.MapFrom(src => src.MST_COMPLAINT_CATEGORY_ID))
                .ForMember(dest => dest.ComplaintCategory, opt => opt.MapFrom(src => src.CATEGORY_NAME))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE));
        }
    }
}
