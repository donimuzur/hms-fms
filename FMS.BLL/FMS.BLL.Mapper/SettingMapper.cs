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
    public class SettingMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_SETTING, SettingDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstSettingId , opt => opt.MapFrom(src => src.MST_SETTING_ID ))
                .ForMember(dest => dest.FunctionGroup , opt => opt.MapFrom(src => src.FUNCTION_GROUP ))
                .ForMember(dest => dest.FunctionName , opt => opt.MapFrom(src => src.FUNCTION_NAME ))
                .ForMember(dest => dest.FunctionValue , opt => opt.MapFrom(src => src.FUNCTION_VALUE ))
                .ForMember(dest => dest.ModifiedBy , opt => opt.MapFrom(src => src.MODIFIED_BY ))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate , opt => opt.MapFrom(src => src.CREATED_DATE  ))
                .ForMember(dest => dest.CreatedBy , opt => opt.MapFrom(src => src.CREATED_BY ))
                .ForMember(dest => dest.IsActive , opt => opt.MapFrom(src => src.IS_ACTIVE ));

            AutoMapper.Mapper.CreateMap<SettingDto, MST_SETTING>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_SETTING_ID, opt => opt.MapFrom(src => src.MstSettingId))
                .ForMember(dest => dest.FUNCTION_GROUP, opt => opt.MapFrom(src => src.FunctionGroup))
                .ForMember(dest => dest.FUNCTION_NAME, opt => opt.MapFrom(src => src.FunctionName))
                .ForMember(dest => dest.FUNCTION_VALUE, opt => opt.MapFrom(src => src.FunctionValue))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

        }
    }
}
