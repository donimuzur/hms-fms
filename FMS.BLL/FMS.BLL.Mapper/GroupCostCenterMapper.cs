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
    public class GroupCostCenterMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_FUNCTION_GROUP, GroupCostCenterDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstFunctionGroupId, opt => opt.MapFrom(src => src.MST_FUNCTION_GROUP_ID))
                .ForMember(dest => dest.FunctionName, opt => opt.MapFrom(src => src.FUNCTION_NAME))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
                .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<GroupCostCenterDto, MST_FUNCTION_GROUP>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MST_FUNCTION_GROUP_ID, opt => opt.MapFrom(src => src.MstFunctionGroupId))
               .ForMember(dest => dest.FUNCTION_NAME, opt => opt.MapFrom(src => src.FunctionName))
               .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
               .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));

            AutoMapper.Mapper.CreateMap<ARCH_MST_FUNCTION_GROUP, GroupCostCenterDto>().IgnoreAllNonExisting()
            .ForMember(dest => dest.MstFunctionGroupId, opt => opt.MapFrom(src => src.MST_FUNCTION_GROUP_ID))
            .ForMember(dest => dest.FunctionName, opt => opt.MapFrom(src => src.FUNCTION_NAME))
            .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
            .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
            .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE));

            AutoMapper.Mapper.CreateMap<GroupCostCenterDto, ARCH_MST_FUNCTION_GROUP>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MST_FUNCTION_GROUP_ID, opt => opt.MapFrom(src => src.MstFunctionGroupId))
               .ForMember(dest => dest.FUNCTION_NAME, opt => opt.MapFrom(src => src.FunctionName))
               .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
               .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive));
        }
    }
}
