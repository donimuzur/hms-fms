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
    public class ReasonMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_REASON, ReasonDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstReasonId, opt => opt.MapFrom(src => src.MST_REASON_ID))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.REASON))
                .ForMember(dest => dest.IsPenalty, opt => opt.MapFrom(src => src.IS_PENALTY))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DOCUMENT_TYPE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
                .ForMember(dest => dest.MstDocumentType, opt => opt.MapFrom(src => src.MST_DOCUMENT_TYPE.DOCUMENT_TYPE));

            AutoMapper.Mapper.CreateMap<ReasonDto, MST_REASON>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_REASON_ID, opt => opt.MapFrom(src => src.MstReasonId))
                .ForMember(dest => dest.REASON, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.IS_PENALTY, opt => opt.MapFrom(src => src.IsPenalty))
                .ForMember(dest => dest.DOCUMENT_TYPE, opt => opt.MapFrom(src => src.DocumentType))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.MST_DOCUMENT_TYPE, opt => opt.MapFrom(src => src.MstDocType));
        }
    }
}
