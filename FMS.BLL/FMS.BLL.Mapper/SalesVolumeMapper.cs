using FMS.AutoMapperExtensions;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;


namespace FMS.BLL.Mapper
{
    public class SalesVolumeMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_SALES_VOLUME, SalesVolumeDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MstSalesVolumeId, opt => opt.MapFrom(src => src.MST_SALES_VOLUME_ID))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TYPE))
               .ForMember(dest => dest.Region, opt => opt.MapFrom(src => src.REGION))
               .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.MONTH))
               .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR))
               .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.VALUE))
               .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
               .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
               .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
               .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
               .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<SalesVolumeDto, MST_SALES_VOLUME>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MST_SALES_VOLUME_ID, opt => opt.MapFrom(src => src.MstSalesVolumeId))
               .ForMember(dest => dest.TYPE, opt => opt.MapFrom(src => src.Type))
               .ForMember(dest => dest.REGION, opt => opt.MapFrom(src => src.Region))
               .ForMember(dest => dest.MONTH, opt => opt.MapFrom(src => src.Month))
               .ForMember(dest => dest.YEAR, opt => opt.MapFrom(src => src.Year))
               .ForMember(dest => dest.VALUE, opt => opt.MapFrom(src => src.Value))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
               .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
