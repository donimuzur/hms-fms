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
                .ForMember(dest => dest.MstComplaintCategoryId, opt => opt.MapFrom(src => src.MST_COMPLAINT_CATEGORY_ID))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CATEGORY_NAME))
                .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.ROLE_TYPE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<ComplaintDto, MST_COMPLAINT_CATEGORY>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MST_COMPLAINT_CATEGORY_ID, opt => opt.MapFrom(src => src.MstComplaintCategoryId))
               .ForMember(dest => dest.CATEGORY_NAME, opt => opt.MapFrom(src => src.CategoryName))
               .ForMember(dest => dest.ROLE_TYPE, opt => opt.MapFrom(src => src.RoleType))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
               .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
