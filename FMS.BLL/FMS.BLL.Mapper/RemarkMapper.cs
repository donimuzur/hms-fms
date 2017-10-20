using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMS.BLL.Mapper
{
    public class RemarkMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_REMARK, RemarkDto>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MstRemarkId, opt => opt.MapFrom(src => src.MST_REMARK_ID))
              .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DOCUMENT_TYPE))
              .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
              .ForMember(dest => dest.RoleType, opt => opt.MapFrom(src => src.ROLE_TYPE))
              .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
              .ForMember(dest => dest.createdDate, opt => opt.MapFrom(src => src.CREATED_DATE))
              .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
              .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
              .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
              .ForMember(dest => dest.MstDocumentType, opt => opt.MapFrom(src => src.MST_DOCUMENT_TYPE.DOCUMENT_TYPE));

            AutoMapper.Mapper.CreateMap<RemarkDto, MST_REMARK>().IgnoreAllNonExisting()
              .ForMember(dest => dest.MST_REMARK_ID, opt => opt.MapFrom(src => src.MstRemarkId))
              .ForMember(dest => dest.DOCUMENT_TYPE, opt => opt.MapFrom(src => src.DocumentType))
              .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark))
              .ForMember(dest => dest.ROLE_TYPE, opt => opt.MapFrom(src => src.RoleType))
              .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
              .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.createdDate))
              .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
              .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
              .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
              .ForMember(dest => dest.MST_DOCUMENT_TYPE, opt => opt.MapFrom(src => src.MstDocumentType));
        }
    }
}
