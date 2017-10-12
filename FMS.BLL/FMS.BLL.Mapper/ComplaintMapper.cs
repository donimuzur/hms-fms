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
            AutoMapper.Mapper.CreateMap<complaint_category, ComplaintDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.CompCatId, opt => opt.MapFrom(src => src.compcat_id))
                .ForMember(dest => dest.ComplaintCategory, opt => opt.MapFrom(src => src.complaint_category1))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.role_id))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.modified_by))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.modified_date));
        }
    }
}
