using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FMS.BusinessObject;
using FMS.BusinessObject.Dto;
using FMS.AutoMapperExtensions;

namespace FMS.BLL.Mapper
{
    public class PenalltyLogicMapper
    {
        public static void Initialize()
        {
            AutoMapper.Mapper.CreateMap<MST_PENALTY_LOGIC, PenaltyLogicDto>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MstPenaltyLogicId, opt => opt.MapFrom(src => src.MST_PENALTY_LOGIC_ID))
               .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.YEAR))
               .ForMember(dest => dest.PenaltyLogic, opt => opt.MapFrom(src => src.PENALTY_LOGIC))
               .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CREATED_BY))
               .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CREATED_DATE))
               .ForMember(dest => dest.ModifiedBy, opt => opt.MapFrom(src => src.MODIFIED_BY))
               .ForMember(dest => dest.ModifiedDate, opt => opt.MapFrom(src => src.MODIFIED_DATE))
               .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IS_ACTIVE))
               .ForMember(dest => dest.MstVendorId, opt => opt.MapFrom(src => src.VENDOR))
               .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VEHICLE_TYPE))
               .ForMember(dest => dest.MstVendor, opt => opt.MapFrom(src => src.MST_VENDOR))
               .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.MST_VENDOR.VENDOR_NAME));

            AutoMapper.Mapper.CreateMap<PenaltyLogicDto, MST_PENALTY_LOGIC>().IgnoreAllNonExisting()
               .ForMember(dest => dest.MST_PENALTY_LOGIC_ID, opt => opt.MapFrom(src => src.MstPenaltyLogicId))
               .ForMember(dest => dest.YEAR, opt => opt.MapFrom(src => src.Year))
               .ForMember(dest => dest.PENALTY_LOGIC, opt => opt.MapFrom(src => src.PenaltyLogic))
               .ForMember(dest => dest.CREATED_BY, opt => opt.MapFrom(src => src.CreatedBy))
               .ForMember(dest => dest.CREATED_DATE, opt => opt.MapFrom(src => src.CreatedDate))
               .ForMember(dest => dest.MODIFIED_BY, opt => opt.MapFrom(src => src.ModifiedBy))
               .ForMember(dest => dest.MODIFIED_DATE, opt => opt.MapFrom(src => src.ModifiedDate))
               .ForMember(dest => dest.IS_ACTIVE, opt => opt.MapFrom(src => src.IsActive))
               .ForMember(dest => dest.MST_VENDOR, opt => opt.MapFrom(src => src.MstVendor))
               .ForMember(dest => dest.VENDOR, opt => opt.MapFrom(src => src.MstVendorId))
               .ForMember(dest => dest.VEHICLE_TYPE, opt => opt.MapFrom(src => src.VehicleType));
        }
    }
}
