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
    public class DocumentTypeMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_DOCUMENT_TYPE, DocumentTypeDto>().IgnoreAllNonExisting()
                 .ForMember(dest => dest.MstDocumentTypeId, opt => opt.MapFrom(src => src.MST_DOCUMENT_TYPE_ID))
                .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DOCUMENT_TYPE))
                .ForMember(dest => dest.DocumentInisial, opt => opt.MapFrom(src => src.DOCUMENT_INISIAL));

            AutoMapper.Mapper.CreateMap<DocumentTypeDto, MST_DOCUMENT_TYPE >().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_DOCUMENT_TYPE_ID, opt => opt.MapFrom(src => src.MstDocumentTypeId ))
               .ForMember(dest => dest.DOCUMENT_TYPE, opt => opt.MapFrom(src => src.DocumentType ))
               .ForMember(dest => dest.DOCUMENT_INISIAL, opt => opt.MapFrom(src => src.DocumentInisial ));
        }
    }
}
