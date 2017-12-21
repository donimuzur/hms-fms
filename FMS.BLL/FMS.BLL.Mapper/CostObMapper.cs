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
    public class CostObMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_COST_OB, CostObDto>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MstCostObId , opt => opt.MapFrom(src => src.MST_COST_OB_ID ))
                .ForMember(dest => dest.Year , opt => opt.MapFrom(src => src.YEAR ))
                .ForMember(dest => dest.Zone , opt => opt.MapFrom(src => src.ZONE ))
                .ForMember(dest => dest.Model , opt => opt.MapFrom(src => src.MODEL ))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.TYPE))
                .ForMember(dest => dest.ObCost, opt => opt.MapFrom(src => src.OB_COST))
                .ForMember(dest => dest.Remark, opt => opt.MapFrom(src => src.REMARK))
                .ForMember(dest => dest.ModifiedBy , opt => opt.MapFrom(src => src.MODIFIED_BY ))
                .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
                .ForMember(dest => dest.CreatedDate , opt => opt.MapFrom(src => src.CREATED_DATE  ))
                .ForMember(dest => dest.CreatedBy , opt => opt.MapFrom(src => src.CREATED_BY ))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
                .ForMember(dest => dest.CostCenter, opt => opt.MapFrom(src => src.COST_CENTER))
                .ForMember(dest => dest.Qty, opt => opt.MapFrom(src => src.QTY))
                .ForMember(dest => dest.Month, opt => opt.MapFrom(src => src.MONTH))
                .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
                .ForMember(dest => dest.FunctionName, opt => opt.MapFrom(src => src.FUNCTION_NAME))
                .ForMember(dest => dest.Regional, opt => opt.MapFrom(src => src.REGIONAL));

            AutoMapper.Mapper.CreateMap<CostObDto, MST_COST_OB>().IgnoreAllNonExisting()
                .ForMember(dest => dest.MST_COST_OB_ID, opt => opt.MapFrom(src => src.MstCostObId))
                .ForMember(dest => dest.YEAR, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.ZONE, opt => opt.MapFrom(src => src.Zone))
                .ForMember(dest => dest.MODEL, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.TYPE, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.OB_COST, opt => opt.MapFrom(src => src.ObCost))
                .ForMember(dest => dest.REMARK, opt => opt.MapFrom(src => src.Remark))
                .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
                .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
                .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.COST_CENTER, opt => opt.MapFrom(src => src.CostCenter))
                .ForMember(dest => dest.QTY, opt => opt.MapFrom(src => src.Qty))
                .ForMember(dest => dest.MONTH, opt => opt.MapFrom(src => src.Month))
                .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType))
                .ForMember(dest => dest.FUNCTION_NAME, opt => opt.MapFrom(src => src.FunctionName))
                .ForMember(dest => dest.REGIONAL, opt => opt.MapFrom(src => src.Regional)); ;

        }
    }
}
