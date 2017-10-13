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
    public class VendorMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_VENDOR, VendorDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstVendorId , opt => opt.MapFrom(src => src.MST_VENDOR_ID ))
                .ForMember(dest => dest.VendorName , opt => opt.MapFrom(src => src.VENDOR_NAME ))
                .ForMember(dest => dest.ShortName , opt => opt.MapFrom(src => src.SHORT_NAME ))
                .ForMember(dest => dest.EmailAddress , opt => opt.MapFrom(src => src.EMAIL_ADDRESS ))
                .ForMember(dest => dest.ModifiedBy , opt => opt.MapFrom(src => src.MODIFIED_BY ))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate , opt => opt.MapFrom(src => src.CREATED_DATE  ))
                .ForMember(dest => dest.CreatedBy , opt => opt.MapFrom(src => src.CREATED_BY ))
                .ForMember(dest => dest.IsActive , opt => opt.MapFrom(src => src.IS_ACTIVE ));

        }
    }
}
